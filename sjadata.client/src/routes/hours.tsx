import { createFileRoute, Link } from "@tanstack/react-router";
import hoursLoader, {
  AreaLabel,
  ParsedHoursCount,
} from "../loaders/hours-loader";
import hoursTargetLoader from "../loaders/hours-target-loader";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faMinus, faPlus, faHouse } from "@fortawesome/free-solid-svg-icons";
import { Loading } from "../components/loading";
import { useEffect, useState } from "react";
import useSelectedAreas from "../components/useSelectedAreas";

interface HoursProps {
  lastMonth: Readonly<ParsedHoursCount>;
  month: Readonly<ParsedHoursCount>;
  monthPlanned: Readonly<ParsedHoursCount>;
  ytd: Readonly<ParsedHoursCount>;
  target: number;
}

export const Route = createFileRoute("/hours")({
  component: function Component() {
    return <Hours {...Route.useLoaderData()} />;
  },
  pendingComponent: Loading,
  loader: async () => ({
    lastMonth: await hoursLoader(
      new Date(new Date().getFullYear(), new Date().getMonth() - 1, 1)
    ),
    month: await hoursLoader(new Date()),
    monthPlanned: await hoursLoader(new Date(), true),
    ytd: await hoursLoader(),
    target: await hoursTargetLoader(new Date()),
  }),
});

const nhseContractAreas: [AreaLabel, string][] = [
  ["NEAS", "North East Ambulance Service"],
  ["NWAS", "North West Ambulance Service"],
  ["WMAS", "West Midlands Ambulance Service"],
  ["EMAS", "East Midlands Ambulance Service"],
  ["EEAST", "East of England Ambulance Service"],
  ["SWAST", "South West Ambulance Service"],
  ["SCAS", "South Central Ambulance Service"],
  ["SECAMB", "South East Coast Ambulance Service"],
  ["LAS", "London Ambulance Service"],
  ["YAS", "Yorkshire Ambulance Service"],
  ["IWAS", "Isle of Wight Ambulance Service"],
];

const regions: [AreaLabel, string][] = [
  ["EM", "East Midlands"],
  ["EOE", "East of England"],
  ["LON", "London"],
  ["NE", "North East"],
  ["NW", "North West"],
  ["SE", "South East"],
  ["SW", "South West"],
  ["WM", "West Midlands"],
];

function calculateSum(
  counts: Partial<Record<AreaLabel, number>>,
  areas: AreaLabel[]
) {
  if (areas == null || areas.length === 0) {
    return Object.values(counts).reduce((acc, val) => acc + val, 0);
  }

  let total = 0;

  for (const area of areas) {
    total += counts[area] ?? 0;
  }

  return total;
}

export function Hours({
  ytd,
  lastMonth,
  month,
  monthPlanned,
  target,
}: HoursProps) {
  const ytdKeys = Object.keys(ytd.counts);

  const actualNhseAreas = nhseContractAreas.filter(([area]) =>
    ytdKeys.includes(area)
  );
  const actualRegions = regions.filter(([area]) => ytdKeys.includes(area));

  const { selectedAreas, AreaCheck, toggleArea } = useSelectedAreas();
  const [expandNHSE, setExpandNHSE] = useState(false);
  const [expandRegions, setExpandRegions] = useState(false);

  useEffect(() => {
    if (selectedAreas && selectedAreas.length > 0) {
      setExpandNHSE(
        selectedAreas.filter((a) =>
          nhseContractAreas.map(([area]) => area).includes(a)
        ).length > 0
      );
      setExpandRegions(
        selectedAreas.filter((a) => regions.map(([area]) => area).includes(a))
          .length > 0
      );
    }
  }, [selectedAreas]);

  const hoursTotal = Math.round(calculateSum(ytd.counts, selectedAreas));
  const lastMonthTotal = Math.round(
    calculateSum(lastMonth.counts, selectedAreas)
  );
  const monthTotal = Math.round(calculateSum(month.counts, selectedAreas));
  const plannedTotal = Math.round(
    calculateSum(monthPlanned.counts, selectedAreas)
  );
  const nhseLastMonthTotal = Math.round(
    calculateSum(
      lastMonth.counts,
      selectedAreas.length === 0
        ? nhseContractAreas.map(([area]) => area)
        : selectedAreas.filter((a) =>
            nhseContractAreas.map(([area]) => area).includes(a)
          )
    )
  );
  const nhseMonthTotal = Math.round(
    calculateSum(
      month.counts,
      selectedAreas.filter((a) =>
        nhseContractAreas.map(([area]) => area).includes(a)
      )
    )
  );
  const plannedNhseMonthTotal = Math.round(
    calculateSum(
      monthPlanned.counts,
      selectedAreas.filter((a) =>
        nhseContractAreas.map(([area]) => area).includes(a)
      )
    )
  );
  const regionsLastMonthTotal = Math.round(
    calculateSum(
      lastMonth.counts,
      selectedAreas.length === 0
        ? nhseContractAreas.map(([area]) => area)
        : selectedAreas.filter((a) => regions.map(([area]) => area).includes(a))
    )
  );
  const regionsMonthTotal = Math.round(
    calculateSum(
      month.counts,
      selectedAreas.filter((a) => regions.map(([area]) => area).includes(a))
    )
  );
  const plannedRegionsMonthTotal = Math.round(
    calculateSum(
      monthPlanned.counts,
      selectedAreas.filter((a) => regions.map(([area]) => area).includes(a))
    )
  );
  const nhseSelected =
    selectedAreas.length === 0 ||
    selectedAreas.filter((a) =>
      nhseContractAreas.map(([area]) => area).includes(a)
    ).length > 0;
  const regionsSelected =
    selectedAreas.length === 0 ||
    selectedAreas.filter((a) => regions.map(([area]) => area).includes(a))
      .length > 0;

  return (
    <>
      <section>
        <h2>Hours</h2>
        <h3>
          <Link to="/">
            <FontAwesomeIcon icon={faHouse} /> Go Home
          </Link>
        </h3>
        {nhseSelected && regionsSelected && (
          <>
            <h3>Volunteer Activity</h3>
            <ul className="link-boxes">
              <li className="hours-box last-month">
                <div>Last Month</div>
                <div>{lastMonthTotal}</div>
              </li>
              <li className="hours-box month">
                <div>This Month</div>
                <div>{monthTotal}</div>
                <div className="planned">{plannedTotal + monthTotal} Planned</div>
              </li>
              <li className="hours-box ytd">
                <div>Year to Date</div>
                <div>{hoursTotal}</div>
              </li>
            </ul>
          </>
        )}
        {nhseSelected && !regionsSelected && (
          <>
            <h3>NHSE Contract</h3>
            <ul className="link-boxes">
              <li className="hours-box last-month">
                <div>Last Month</div>
                <div>{nhseLastMonthTotal}</div>
              </li>
              <li className="hours-box month">
                <div>This Month</div>
                <div>{nhseMonthTotal}</div>
                <div className="planned">{plannedNhseMonthTotal + nhseMonthTotal} Planned</div>
              </li>
              <li className="hours-box target">
                <div>Target</div>
                <div>{target}</div>
              </li>
            </ul>
          </>
        )}
        {!nhseSelected && regionsSelected && (
          <>
            <h3>Events</h3>
            <ul className="link-boxes">
              <li className="hours-box last-month">
                <div>Last Month</div>
                <div>{regionsLastMonthTotal}</div>
              </li>
              <li className="hours-box month">
                <div>This Month</div>
                <div>{regionsMonthTotal}</div>
                <div className="planned">{plannedRegionsMonthTotal + regionsMonthTotal} Planned</div>
              </li>
            </ul>
          </>
        )}
      </section>
      <section>
        <h3>Filter</h3>
        <ul className="area-list">
          <li className="area-section" onClick={() => setExpandNHSE((s) => !s)}>
            {expandNHSE ? (
              <FontAwesomeIcon icon={faMinus} />
            ) : (
              <FontAwesomeIcon icon={faPlus} />
            )}{" "}
            NHSE Contract
          </li>
          <li>
            <ul
              className={"area-section-list" + (expandNHSE ? " expanded" : "")}
            >
              {actualNhseAreas.map(([area, description]) => (
                <li key={area} onClick={() => toggleArea(area)}>
                  <AreaCheck area={area} /> {description}
                </li>
              ))}
            </ul>
          </li>
          <li
            className="area-section"
            onClick={() => setExpandRegions((s) => !s)}
          >
            {expandRegions ? (
              <FontAwesomeIcon icon={faMinus} />
            ) : (
              <FontAwesomeIcon icon={faPlus} />
            )}{" "}
            Event Regions
          </li>
          <li>
            <ul
              className={
                "area-section-list" + (expandRegions ? " expanded" : "")
              }
            >
              {actualRegions.map(([area, description]) => (
                <li key={area} onClick={() => toggleArea(area)}>
                  <AreaCheck area={area} /> {description}
                </li>
              ))}
            </ul>
          </li>
        </ul>
        <footer>
          <p>
            Hours are all shown as people-hours, not crew-hours (and so are
            double what we bill to NHSE).
          </p>
          <p className="last-update">
            Data last updated : {ytd.lastUpdate?.toLocaleString() ?? "No Data"}{" "}
          </p>
        </footer>
      </section>
    </>
  );
}
