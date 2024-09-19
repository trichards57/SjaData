import { faHouse } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { createFileRoute, Link } from "@tanstack/react-router";
import { Regions, regionToString } from "../loaders/hours-loader";
import { LinkBox, LinkBoxes } from "../components/link-boxes";
import { Loading } from "../components/loading";

export function TrendsPage() {
  const sortedRegions = [...Regions].sort((a, b) => a.localeCompare(b));

  return (
    <>
      <h2>Home</h2>
      <h3>
        <Link to="/">
          <FontAwesomeIcon icon={faHouse} /> Go Home
        </Link>
      </h3>
      <LinkBoxes size="small">
        {sortedRegions.map((r) => (
          <LinkBox
            key="r"
            color="green"
            to="/trends/$region"
            params={{ region: "NE" }}
          >
            {regionToString(r)}
          </LinkBox>
        ))}
      </LinkBoxes>
    </>
  );
}

export const Route = createFileRoute("/trends-menu")({
  pendingComponent: Loading,
  component: TrendsPage,
});
