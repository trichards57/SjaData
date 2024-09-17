import { createFileRoute, Link } from "@tanstack/react-router";
import {
  AreaLabel,
  preloadHoursCount,
  useHoursCount,
} from "../loaders/hours-loader";
import {
  preloadHoursTargetCount,
  useHoursTarget,
} from "../loaders/hours-target-loader";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faMinus, faPlus, faHouse } from "@fortawesome/free-solid-svg-icons";
import { Loading } from "../components/loading";
import { useEffect, useState } from "react";
import useSelectedAreas from "../components/useSelectedAreas";
import { addMonths } from "date-fns";
import { LinkBox, LinkBoxes } from "../components/link-boxes";

export const Route = createFileRoute("/hours")({
  component: Hours,
  pendingComponent: Loading,
  loader: async ({ context }) => {
    return Promise.all([
      preloadHoursCount(
        context.queryClient,
        context.pca,
        new Date(new Date().getFullYear(), new Date().getMonth() - 1, 1)
      ),
      preloadHoursCount(context.queryClient, context.pca, new Date()),
      preloadHoursCount(context.queryClient, context.pca, new Date(), true),
      preloadHoursCount(context.queryClient, context.pca),
      preloadHoursTargetCount(context.queryClient, context.pca, new Date()),
    ]);
  },
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

export function Hours() {
  const target = useHoursTarget(new Date());
  const lastMonth = useHoursCount(addMonths(new Date(), -1));
  const month = useHoursCount(new Date());
  const monthPlanned = useHoursCount(new Date(), true);
  const ytd = useHoursCount();
  const ytdKeys = Object.keys(ytd.data.counts);

  const actualNhseAreas = nhseContractAreas.filter(([area]) =>
    ytdKeys.includes(area)
  );
  const actualRegions = regions.filter(([area]) => ytdKeys.includes(area));

  const { selectedAreas, AreaCheck, toggleArea, setAreas } = useSelectedAreas();
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

  const hoursTotal = Math.round(calculateSum(ytd.data.counts, selectedAreas));
  const lastMonthTotal = Math.round(
    calculateSum(lastMonth.data.counts, selectedAreas)
  );
  const monthTotal = Math.round(calculateSum(month.data.counts, selectedAreas));
  const plannedTotal = Math.round(
    calculateSum(monthPlanned.data.counts, selectedAreas)
  );
  const nhseLastMonthTotal = Math.round(
    calculateSum(
      lastMonth.data.counts,
      selectedAreas.length === 0
        ? nhseContractAreas.map(([area]) => area)
        : selectedAreas.filter((a) =>
            nhseContractAreas.map(([area]) => area).includes(a)
          )
    )
  );
  const nhseMonthTotal = Math.round(
    calculateSum(
      month.data.counts,
      selectedAreas.filter((a) =>
        nhseContractAreas.map(([area]) => area).includes(a)
      )
    )
  );
  const plannedNhseMonthTotal = Math.round(
    calculateSum(
      monthPlanned.data.counts,
      selectedAreas.filter((a) =>
        nhseContractAreas.map(([area]) => area).includes(a)
      )
    )
  );
  const regionsLastMonthTotal = Math.round(
    calculateSum(
      lastMonth.data.counts,
      selectedAreas.length === 0
        ? nhseContractAreas.map(([area]) => area)
        : selectedAreas.filter((a) => regions.map(([area]) => area).includes(a))
    )
  );
  const regionsMonthTotal = Math.round(
    calculateSum(
      month.data.counts,
      selectedAreas.filter((a) => regions.map(([area]) => area).includes(a))
    )
  );
  const plannedRegionsMonthTotal = Math.round(
    calculateSum(
      monthPlanned.data.counts,
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

  function selectAllNhse(e: React.MouseEvent) {
    e.stopPropagation();
    setAreas(actualNhseAreas.map(([a]) => a));
  }

  function selectAllEvents(e: React.MouseEvent) {
    e.stopPropagation();
    setAreas(actualRegions.map(([a]) => a));
  }

  function clearAll(e: React.MouseEvent) {
    e.stopPropagation();
    setAreas([]);
    setExpandNHSE(false);
    setExpandRegions(false);
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
        {nhseSelected && regionsSelected && (
          <>
            <h3>Volunteer Activity</h3>
            <LinkBoxes size="large">
              <LinkBox color="light-gray">
                <div>Last Month</div>
                <div>{lastMonthTotal}</div>
              </LinkBox>
              <LinkBox color="black">
                <div>This Month</div>
                <div>{monthTotal}</div>
                <div className="planned">
                  {plannedTotal + monthTotal} Planned
                </div>
              </LinkBox>
              <LinkBox color="dark-green">
                <div>Year to Date</div>
                <div>{hoursTotal}</div>
              </LinkBox>
            </LinkBoxes>
          </>
        )}
        {nhseSelected && !regionsSelected && (
          <>
            <h3>NHSE Contract</h3>
            <LinkBoxes size="large">
              <LinkBox color="light-gray">
                <div>Last Month</div>
                <div>{nhseLastMonthTotal}</div>
              </LinkBox>
              <LinkBox color="black">
                <div>This Month</div>
                <div>{nhseMonthTotal}</div>
                <div className="planned">
                  {plannedNhseMonthTotal + nhseMonthTotal} Planned
                </div>
              </LinkBox>
              <LinkBox color="dark-green">
                <div>Target</div>
                <div>{target.data.target}</div>
              </LinkBox>
            </LinkBoxes>
          </>
        )}
        {!nhseSelected && regionsSelected && (
          <>
            <h3>Events</h3>
            <LinkBoxes size="large">
              <LinkBox color="light-gray">
                <div>Last Month</div>
                <div>{regionsLastMonthTotal}</div>
              </LinkBox>
              <LinkBox color="black">
                <div>This Month</div>
                <div>{regionsMonthTotal}</div>
                <div className="planned">
                  {plannedRegionsMonthTotal + regionsMonthTotal} Planned
                </div>
              </LinkBox>
            </LinkBoxes>
          </>
        )}
      </section>
      <section className="filter-section">
        <h3>
          Filter
          <button onClick={clearAll}>Clear</button>
        </h3>
        <ul className="area-list">
          <li className="area-section" onClick={() => setExpandNHSE((s) => !s)}>
            <span>
              {expandNHSE ? (
                <FontAwesomeIcon icon={faMinus} />
              ) : (
                <FontAwesomeIcon icon={faPlus} />
              )}{" "}
              NHSE Contract
            </span>
            <span>
              <button onClick={selectAllNhse}>Select All</button>
            </span>
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
            <span>
              {expandRegions ? (
                <FontAwesomeIcon icon={faMinus} />
              ) : (
                <FontAwesomeIcon icon={faPlus} />
              )}{" "}
              Event Regions
            </span>
            <span>
              <button onClick={selectAllEvents}>Select All</button>
            </span>
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
            Data last updated :{" "}
            {ytd.data.lastUpdate?.toLocaleString() ?? "No Data"}{" "}
          </p>
        </footer>
      </section>
    </>
  );
}
