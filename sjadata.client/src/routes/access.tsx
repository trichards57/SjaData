import { createFileRoute } from "@tanstack/react-router";
import { Loading } from "../components/loading";
import { preloadUsers, useUsers } from "../loaders/users-loader";
import { useMsal } from "@azure/msal-react";

export function UserAccess() {
  const { data: users } = useUsers();
  const msal = useMsal();
  const userId = msal.instance.getActiveAccount()?.idTokenClaims?.sub;

  console.log(msal.instance.getActiveAccount());

  return (
    <>
      <h2>Users</h2>
      <table>
        <thead>
          <tr>
            <th />
            <th>Role</th>
          </tr>
        </thead>
        <tbody>
          {users.map((user) => (
            <tr key={user.id}>
              <td>{user.name}</td>
              <td>
                <button
                  disabled={user.id === userId}
                  className={user.role === "Admin" ? "checked" : ""}
                >
                  Admin
                </button>
                <button
                  disabled={user.id === userId}
                  className={user.role === "Lead" ? "checked" : ""}
                >
                  Lead
                </button>
                <button
                  disabled={user.id === userId}
                  className={user.role === "None" ? "checked" : ""}
                >
                  User
                </button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </>
  );
}

export const Route = createFileRoute("/access")({
  pendingComponent: Loading,
  loader: ({ context }) => {
    return preloadUsers(context.queryClient, context.pca);
  },
  component: UserAccess,
});
