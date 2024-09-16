import { createFileRoute, Link, notFound } from "@tanstack/react-router";
import { Region, Regions, regionToString } from "../loaders/hours-loader";
import {
  PersonReport,
  preloadPeopleReports,
  usePeopleReports,
} from "../loaders/people-report-loader";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faBackward } from "@fortawesome/free-solid-svg-icons";
import styles from "./trends.module.css";
import { Line, LineChart, ResponsiveContainer } from "recharts";

interface PeopleReportsPageProps {
  region: Region;
}

function TrendRow({ report }: { report: Readonly<PersonReport> }) {
  return (
    <tr>
      <td>{report.name}</td>
      <td>
        <div>{Math.round(report.hoursThisYear)}</div>
      </td>
      <td>
        <ResponsiveContainer
          width={100}
          height={40}
          className={styles["line-chart"]}
        >
          <LineChart
            width={300}
            height={100}
            data={report.hours.map((h) => ({
              hours: h,
            }))}
          >
            <Line dataKey="hours" stroke="black" strokeWidth={2} dot={false} />
          </LineChart>
        </ResponsiveContainer>
      </td>
    </tr>
  );
}

function GroupRow({
  title,
  reports,
}: {
  title: string;
  reports: Readonly<PersonReport>[];
}) {
  return (
    <>
      <h3>{title}</h3>
      <table className={styles["activity-trends"]}>
        <thead>
          <tr>
            <th>Name</th>
            <th>Hours this Year</th>
            <th>Last 12 Months</th>
          </tr>
        </thead>
        <tbody>
          {reports.map((person) => (
            <TrendRow key={person.name} report={person} />
          ))}
        </tbody>
      </table>
    </>
  );
}

export function PeopleReportsPage({ region }: PeopleReportsPageProps) {
  const { data: reports } = usePeopleReports(region);

  const sortedReports = [...reports]?.sort(
    (a, b) => b.hoursThisYear - a.hoursThisYear
  );

  const activePeople = sortedReports.filter(
    (p) => p.monthsSinceLastActive <= 3
  );
  const dormantPeople = sortedReports.filter(
    (p) => p.monthsSinceLastActive > 3 && p.monthsSinceLastActive <= 6
  );
  const inactivePeople = sortedReports.filter(
    (p) => p.monthsSinceLastActive > 6 && p.monthsSinceLastActive <= 9
  );
  const retiredPeople = sortedReports.filter(
    (p) => p.monthsSinceLastActive > 9
  );

  return (
    <>
      <section>
        <h2>People Activity - {regionToString(region)}</h2>
        <h3>
          <Link to="/people-menu">
            <FontAwesomeIcon icon={faBackward} /> Go Back
          </Link>
        </h3>
        <GroupRow
          title={`Active People (${activePeople.length}) (<3 months inactive)`}
          reports={activePeople}
        />
        <GroupRow
          title={`Dormant People (${dormantPeople.length}) (4-6 months inactive)`}
          reports={dormantPeople}
        />
        <GroupRow
          title={`Inactive People (${inactivePeople.length}) (7-9 months inactive)`}
          reports={inactivePeople}
        />
        <GroupRow
          title={`Retired People (${retiredPeople.length}) (>9 months inactive)`}
          reports={retiredPeople}
        />
      </section>
      <footer></footer>
    </>
  );
}

export const Route = createFileRoute("/people/$region")({
  beforeLoad: async (ctx) => {
    const { region } = ctx.params;
    if (!Regions.includes(region as Region)) {
      throw notFound();
    }
  },
  loader: async ({ context, params }) => {
    return preloadPeopleReports(
      context.queryClient,
      context.pca,
      params.region as Region
    );
  },
  component: function Component() {
    const { region } = Route.useParams();
    return <PeopleReportsPage region={region as Region} />;
  },
});
