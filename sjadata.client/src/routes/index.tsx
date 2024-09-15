import { createFileRoute, Link } from "@tanstack/react-router";
import { preloadMe, useMe } from "../loaders/user-loader";

export const Route = createFileRoute("/")({
  component: Index,
  loader: async ({ context }) => {
    const tokenRes = await context.pca.acquireTokenSilent({
      scopes: ["User.Read"],
      account: context.pca.getAccount({
        tenantId: "91d037fb-4714-4fe8-b084-68c083b8193f",
      })!,
    });
    const token = tokenRes.idToken;
    preloadMe(context.queryClient, token);
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
          <Link className="link-box" to="/trends">
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
