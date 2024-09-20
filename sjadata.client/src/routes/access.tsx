import { createFileRoute, Link } from "@tanstack/react-router";
import { Loading } from "../components/loading";
import { preloadUsers, useUpdateRole, useUsers } from "../loaders/users-loader";
import { useMsal } from "@azure/msal-react";
import styles from "./access.module.css";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faHouse } from "@fortawesome/free-solid-svg-icons";

export function UserAccess() {
  const { data: users } = useUsers();
  const msal = useMsal();
  const userId = msal.instance.getActiveAccount()?.idTokenClaims?.sub;
  const { mutateAsync: updateRole, status: mutateStatus } = useUpdateRole();

  async function changeRole(id: string, role: string) {
    if (id === userId) return;

    await updateRole({ id, role });
  }

  const sortedUsers = [...users].sort((a, b) => a.name.localeCompare(b.name));

  return (
    <>
      <h2>Users</h2>
      <h3>
        <Link to="/">
          <FontAwesomeIcon icon={faHouse} /> Go Home
        </Link>
      </h3>
      <table className={styles["access-table"]}>
        <thead>
          <tr>
            <th />
            <th>Role</th>
          </tr>
        </thead>
        <tbody>
          {sortedUsers.map((user) => (
            <tr key={user.id}>
              <td>{user.name}</td>
              <td>
                {["Admin", "Lead", "None"].map((role) => (
                  <button
                    disabled={user.id === userId || mutateStatus === "pending"}
                    className={user.role === role ? styles["checked"] : ""}
                    title={
                      user.id === userId
                        ? "You cannot change your own role."
                        : ""
                    }
                    aria-checked={user.role === role ? "true" : "false"}
                    onClick={() => changeRole(user.id, role)}
                  >
                    {role}
                  </button>
                ))}
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
