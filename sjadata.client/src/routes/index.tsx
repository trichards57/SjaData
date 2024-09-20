import { createFileRoute } from "@tanstack/react-router";
import { preloadMe, useMe } from "../loaders/user-loader";
import { LinkBox, LinkBoxes } from "../components/link-boxes";
import { Loading } from "../components/loading";

export const Route = createFileRoute("/")({
  component: Index,
  pendingComponent: Loading,
  loader: async ({ context }) => {
    preloadMe(context.queryClient, context.pca);
  },
});

function Index() {
  const { data: me } = useMe();

  return (
    <>
      <h2>Home</h2>
      <LinkBoxes size="small">
        <LinkBox isLink color="green" to="/hours">
          Hours
        </LinkBox>
        {(me.role === "Lead" || me.role === "Admin") && (
          <>
            <LinkBox isLink color="green" to="/trends-menu">
              Trends
            </LinkBox>
            <LinkBox isLink color="green" to="/people">
              People
            </LinkBox>
          </>
        )}
        {me.role === "Admin" && (
          <>
            <LinkBox isLink color="dark-green" to="/update">
              Update
            </LinkBox>
            <LinkBox isLink color="yellow" to="/access">
              Access
            </LinkBox>
          </>
        )}
      </LinkBoxes>
    </>
  );
}
