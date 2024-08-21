import { createFileRoute } from "@tanstack/react-router";
import hoursLoader from "../loaders/hours-loader";
import hoursTargetLoader from "../loaders/hours-target-loader";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faPlus } from "@fortawesome/free-solid-svg-icons";

export const Route = createFileRoute("/hours")({
  component: Hours,
  loader: async () => ({
    month: await hoursLoader(new Date()),
    ytd: await hoursLoader(),
    target: await hoursTargetLoader(new Date()),
  }),
});

function Hours() {
  const data = Route.useLoaderData();

  const hoursTotal = Math.round(
    Object.values(data.ytd.counts).reduce((acc, val) => acc + val, 0)
  );

  const monthTotal = Math.round(
    Object.values(data.month.counts).reduce((acc, val) => acc + val, 0)
  );

  return (
    <>
      <section>
        <h2>Hours</h2>
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
            <div>{data.target}</div>
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
          <li>
            <FontAwesomeIcon icon={faPlus} />
            NHSE Contract
          </li>
          <ul>
            <li>North East Ambulance Service</li>
            <li>North West Ambulance Service</li>
            <li>West Midlands Ambulance Service</li>
            <li>East Midlands Ambulance Service</li>
            <li>East of England Ambulance Service</li>
            <li>South West Ambulance Service</li>
            <li>South Central Ambulance Service</li>
            <li>South East Coast Ambulance Service</li>
            <li>London Ambulance Service</li>
            <li>Yorkshire Ambulance Service</li>
            <li>Isle of Wight Ambulance Service</li>
          </ul>
        </ul>
      </section>
      <p>Data last updated : {data.ytd.lastUpdate.toLocaleString()} </p>
    </>
  );
}
