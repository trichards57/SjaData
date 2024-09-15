import { faHouse } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { createFileRoute, Link } from "@tanstack/react-router";

export function TrendsPage() {
  return (
    <>
      <h2>Home</h2>
      <h3>
        <Link to="/">
          <FontAwesomeIcon icon={faHouse} /> Go Home
        </Link>
      </h3>
      <div className="link-boxes">
        <Link
          className="link-box"
          to="/trends/$region"
          params={{ region: "NE" }}
        >
          North East
        </Link>
        <Link
          className="link-box"
          to="/trends/$region"
          params={{ region: "NW" }}
        >
          North West
        </Link>
        <Link
          className="link-box"
          to="/trends/$region"
          params={{ region: "EM" }}
        >
          East Midlands
        </Link>
        <Link
          className="link-box"
          to="/trends/$region"
          params={{ region: "WM" }}
        >
          West Midlands
        </Link>
        <Link
          className="link-box"
          to="/trends/$region"
          params={{ region: "EOE" }}
        >
          East of England
        </Link>
        <Link
          className="link-box"
          to="/trends/$region"
          params={{ region: "LON" }}
        >
          London
        </Link>
        <Link
          className="link-box"
          to="/trends/$region"
          params={{ region: "SW" }}
        >
          South West
        </Link>
        <Link
          className="link-box"
          to="/trends/$region"
          params={{ region: "SE" }}
        >
          South East
        </Link>
      </div>
    </>
  );
}

export const Route = createFileRoute("/trends-menu")({
  component: TrendsPage,
});
