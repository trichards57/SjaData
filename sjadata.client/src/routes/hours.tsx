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

export function Hours({ ytd, lastMonth, month, target }: HoursProps) {
  const ytdKeys = Object.keys(ytd.counts);

  const actualNhseAreas = nhseContractAreas.filter(([area]) =>
    ytdKeys.includes(area)
  );

  const { selectedAreas, AreaCheck, toggleArea } = useSelectedAreas();
  const [expandNHSE, setExpandNHSE] = useState(false);

  useEffect(() => {
    if (selectedAreas && selectedAreas.length > 0) {
      setExpandNHSE(true);
    }
  }, [selectedAreas]);

  const hoursTotal = Math.round(calculateSum(ytd.counts, selectedAreas));
  const lastMonthTotal = Math.round(
    calculateSum(lastMonth.counts, selectedAreas)
  );
  const monthTotal = Math.round(calculateSum(month.counts, selectedAreas));

  return (
    <>
      <section>
        <h2>Hours</h2>
        <h3>
          <Link to="/">
            <FontAwesomeIcon icon={faHouse} /> Go Home
          </Link>
        </h3>
        <ul className="link-boxes">
          <li className="hours-box month">
            <div>Last Month</div>
            <div>{lastMonthTotal}</div>
          </li>
          <li className="hours-box month">
            <div>This Month</div>
            <div>{monthTotal}</div>
          </li>
          <li className="hours-box target">
            <div>NHSE Target</div>
            <div>{target}</div>
          </li>
          <li className="hours-box ytd">
            <div>Year to Date</div>
            <div>{hoursTotal}</div>
          </li>
        </ul>
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
          <ul className={"area-section-list" + (expandNHSE ? " expanded" : "")}>
            {actualNhseAreas.map(([area, description]) => (
              <li key={area} onClick={() => toggleArea(area)}>
                <AreaCheck area={area} /> {description}
              </li>
            ))}
          </ul>
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
