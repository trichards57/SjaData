import { faHouse } from '@fortawesome/free-solid-svg-icons'
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'
import { createFileRoute, Link } from '@tanstack/react-router'
import { LinkBox, LinkBoxes } from '../../components/link-boxes'
import { Regions, regionToString } from '../../loaders/hours-loader'

export function PeoplePage() {
  const sortedRegions = [...Regions].sort((a, b) => a.localeCompare(b))

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
            key={r}
            color="green"
            to="/people/$region"
            params={{ region: r }}
          >
            {regionToString(r)}
          </LinkBox>
        ))}
      </LinkBoxes>
    </>
  )
}

export const Route = createFileRoute('/people/')({
  component: PeoplePage,
})
