import { createFileRoute, Link } from "@tanstack/react-router";
import hoursLoader, {
  AreaLabel,
  ParsedHoursCount,
} from "../loaders/hours-loader";
import hoursTargetLoader from "../loaders/hours-target-loader";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import {
  faCheck,
  faMinus,
  faPlus,
  faXmark,
  faHouse,
} from "@fortawesome/free-solid-svg-icons";
import { Loading } from "../components/loading";
import { useState } from "react";

interface HoursProps {
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

export function Hours({ ytd, month, target }: HoursProps) {
  const ytdKeys = Object.keys(ytd.counts);

  const actualNhseAreas = nhseContractAreas.filter(([area]) =>
    ytdKeys.includes(area)
  );

  const [expandNHSE, setExpandNHSE] = useState(false);
  const [selectedAreas, setSelectedAreas] = useState<AreaLabel[]>([]);

  const hoursTotal = Math.round(calculateSum(ytd.counts, selectedAreas));
  const monthTotal = Math.round(calculateSum(month.counts, selectedAreas));

  function AreaCheck({ area }: { area: AreaLabel }) {
    if (selectedAreas.includes(area)) {
      return <FontAwesomeIcon fixedWidth icon={faCheck} />;
    } else {
      return <FontAwesomeIcon fixedWidth icon={faXmark} />;
    }
  }

  function toggleArea(area: AreaLabel) {
    setSelectedAreas((areas) =>
      areas.includes(area) ? areas.filter((a) => a !== area) : [...areas, area]
    );
  }

  return (
    <>
      <section>
        <h2>Hours</h2>
        <h3>
          <Link to="/">
            <FontAwesomeIcon icon={faHouse} /> Go Home
          </Link>
        </h3>
        <div className="link-boxes">
          <div className="hours-box month">
            <div>This Month</div>
            <div>{monthTotal}</div>
          </div>
          <div className="hours-box ytd">
            <div>Year to Date</div>
            <div>{hoursTotal}</div>
          </div>
          <div className="hours-box target">
            <div>
              NHSE Target
              <a className="see" href="#target-note">
                [*]
              </a>
            </div>
            <div>{target}</div>
          </div>
        </div>
        <p>
          <a id="target-note">*</a> Target is shown as people-hours, not
          crew-hours (and so is double what we bill to NHSE).
        </p>
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
      </section>
      <p className="last-update">
        Data last updated : {ytd.lastUpdate?.toLocaleString() ?? "No Data"}{" "}
      </p>
    </>
  );
}
