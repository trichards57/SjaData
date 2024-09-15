import { createFileRoute, Link } from "@tanstack/react-router";
import { preloadMe, useMe } from "../loaders/user-loader";

export const Route = createFileRoute("/")({
  component: Index,
  loader: async ({ context }) => {
    preloadMe(context.queryClient, context.pca);
  },
});

function Index() {
  const { data: me } = useMe();

  return (
    <>
      <h2>Home</h2>
      <div className="link-boxes">
        <Link className="link-box" to="/hours">
          Hours
        </Link>
        {(me.role === "Lead" || me.role === "Admin") && (
          <Link className="link-box" to="/trends-menu">
            Trends
          </Link>
        )}
        {me.role === "Admin" && (
          <>
            <Link className="link-box" to="/update">
              Update
            </Link>
            <Link className="link-box" to="/access">
              Access
            </Link>
          </>
        )}
      </div>
    </>
  );
}
