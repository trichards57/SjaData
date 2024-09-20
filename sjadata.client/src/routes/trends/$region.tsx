import { createFileRoute, Link, notFound } from '@tanstack/react-router'
import styles from '../trends.module.css'
import { format } from 'date-fns'
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'
import {
  faArrowDown,
  faArrowUp,
  faBackward,
  faEquals,
} from '@fortawesome/free-solid-svg-icons'
import { preloadTrends, Trends, useTrends } from '../../loaders/trends-loader'
import { Region, Regions, regionToString } from '../../loaders/hours-loader'
import { Line, LineChart, ResponsiveContainer } from 'recharts'
import { Loading } from '../../components/loading'

const significantChange = 0.05 // 5%
const significantHours = 24

function getArrow(value: number, oldValue: number) {
  const change = oldValue - value
  const diff = (oldValue - value) / oldValue

  if (change > 24 && diff > significantChange) {
    return (
      <i
        className={styles['drop']}
        title={`Down ${diff.toLocaleString(undefined, { style: 'percent', minimumFractionDigits: 1 })}`}
      >
        <FontAwesomeIcon fixedWidth icon={faArrowDown} />
      </i>
    )
  } else if (change < 24 && diff < -significantChange) {
    return (
      <i
        className={styles['increase']}
        title={`Up ${(-diff).toLocaleString(undefined, { style: 'percent', minimumFractionDigits: 1 })}`}
      >
        <FontAwesomeIcon fixedWidth icon={faArrowUp} />
      </i>
    )
  } else {
    const changeStr =
      Math.abs(change) > significantHours
        ? (-diff).toLocaleString(undefined, {
            style: 'percent',
            minimumFractionDigits: 1,
          })
        : `${Math.round(-change)} hours`
    return (
      <i className={styles['no-change']} title={`Change ${changeStr}`}>
        <FontAwesomeIcon fixedWidth icon={faEquals} />
      </i>
    )
  }
}

function TrendRow({
  trends,
  keyValue,
  label,
}: {
  trends: Readonly<Trends>
  keyValue: string
  label: string
}) {
  if (!trends.twelveMonthAverage[keyValue]) {
    return null
  }

  return (
    <tr>
      <td>{label}</td>
      <td>
        <div>
          {+trends.twelveMonthAverage[keyValue].toPrecision(2)}
          {getArrow(
            trends.twelveMonthAverage[keyValue],
            trends.twelveMonthMinusOneAverage[keyValue],
          )}
        </div>
      </td>
      <td>
        <div>
          {+trends.sixMonthAverage[keyValue].toPrecision(2)}
          {getArrow(
            trends.sixMonthAverage[keyValue],
            trends.sixMonthMinusOneAverage[keyValue],
          )}
        </div>
      </td>
      <td>
        <div>
          {+trends.threeMonthAverage[keyValue].toPrecision(2)}
          {getArrow(
            trends.threeMonthAverage[keyValue],
            trends.threeMonthMinusOneAverage[keyValue],
          )}
        </div>
      </td>
      <td>
        <ResponsiveContainer
          width={100}
          height={40}
          className={styles['line-chart']}
        >
          <LineChart
            width={300}
            height={100}
            data={trends.hours[keyValue].map((h) => ({
              hours: h,
            }))}
          >
            <Line dataKey="hours" stroke="black" strokeWidth={2} dot={false} />
          </LineChart>
        </ResponsiveContainer>
      </td>
    </tr>
  )
}

interface TrendsPageProps {
  region: Region
}

export function TrendsPage({ region }: TrendsPageProps) {
  const { data: trends } = useTrends(region)
  const { data: nhseTrends } = useTrends(region, true)

  const districts = Object.keys(trends?.twelveMonthAverage ?? {}).filter(
    (s) => s != 'national' && s != 'region',
  )
  const nhseDistricts = Object.keys(
    nhseTrends?.twelveMonthAverage ?? {},
  ).filter((s) => s != 'national' && s != 'region')

  return (
    <>
      <section>
        <h2>Trends</h2>
        <h3>
          <Link to="/trends">
            <FontAwesomeIcon icon={faBackward} /> Go Back
          </Link>
        </h3>
        <h3>All Activity</h3>
        <table className={styles['activity-trends']}>
          <thead>
            <tr>
              <th>Area</th>
              <th>12 Month Avg</th>
              <th>6 Month Avg</th>
              <th>3 Month Avg</th>
              <th>Last 12 Months</th>
            </tr>
          </thead>
          <tbody>
            <TrendRow trends={trends} keyValue="national" label="National" />
            <TrendRow
              trends={trends}
              keyValue="region"
              label={regionToString(region)}
            />
            {districts.map((district) => (
              <TrendRow
                key={district}
                trends={trends}
                keyValue={district}
                label={district}
              />
            ))}
          </tbody>
        </table>
        <h3>NHSE Activity</h3>
        <table className={styles['activity-trends']}>
          <thead>
            <tr>
              <th>Area</th>
              <th>12 Month Avg</th>
              <th>6 Month Avg</th>
              <th>3 Month Avg</th>
              <th>Last 12 Months</th>
            </tr>
          </thead>
          <tbody>
            <TrendRow
              trends={nhseTrends}
              keyValue="national"
              label="National"
            />
            <TrendRow
              trends={nhseTrends}
              keyValue="region"
              label={regionToString(region)}
            />
            {nhseDistricts.map((district) => (
              <TrendRow
                key={district}
                trends={nhseTrends}
                keyValue={district}
                label={district}
              />
            ))}
          </tbody>
        </table>
      </section>
      <footer>
        <p>
          Hours are all shown as people-hours, to two-significant figures. A
          change of less than{' '}
          {significantChange.toLocaleString(undefined, { style: 'percent' })} or{' '}
          {significantHours} hours (whichever is greater) is considered
          insignificant.
        </p>
        <p className="last-update">
          Data up until {format(trends.thresholdDate, 'PP')}
        </p>
      </footer>
    </>
  )
}

export const Route = createFileRoute('/trends/$region')({
  pendingComponent: Loading,
  beforeLoad: async (ctx) => {
    const { region } = ctx.params
    if (!Regions.includes(region as Region)) {
      throw notFound()
    }
  },
  loader: async ({ context, params }) => {
    return Promise.all([
      preloadTrends(context.queryClient, context.pca, params.region as Region),
      preloadTrends(
        context.queryClient,
        context.pca,
        params.region as Region,
        true,
      ),
    ])
  },
  component: function Component() {
    const { region } = Route.useParams()
    return <TrendsPage region={region as Region} />
  },
})
