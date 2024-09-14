import { faHouse } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { createFileRoute, Link } from "@tanstack/react-router";

export function Trends() {
  return (
    <>
      <section>
        <h2>Trends</h2>
        <h3>
          <Link to="/">
            <FontAwesomeIcon icon={faHouse} /> Go Home
          </Link>
        </h3>
        <h3>All Activity</h3>
        <table className="activity-trends">
          <thead>
            <tr>
              <th />
              <th>12 Month</th>
              <th>6 Month</th>
              <th>3 Month</th>
              <th />
            </tr>
          </thead>
          <tbody>
            <tr>
                <td>National</td>
            </tr>
            <tr>
                <td>Region</td>
            </tr>
            <tr>
                <td>Districts</td>
            </tr>
          </tbody>
        </table>
        <h3>NHSE Activity</h3>
        <table className="activity-trends">
          <thead>
            <tr>
              <th />
              <th>12 Month</th>
              <th>6 Month</th>
              <th>3 Month</th>
              <th />
            </tr>
          </thead>
          <tbody>
            <tr>
                <td>National</td>
            </tr>
            <tr>
                <td>Region</td>
            </tr>
            <tr>
                <td>Districts</td>
            </tr>
          </tbody>
        </table>
      </section>
    </>
  );
}

export const Route = createFileRoute("/trends")({
  component: Trends,
});
