import { IPublicClientApplication } from "@azure/msal-browser";
import { useMsal } from "@azure/msal-react";
import {
  QueryClient,
  QueryKey,
  queryOptions,
  useMutation,
  useQueryClient,
  useSuspenseQuery,
} from "@tanstack/react-query";
import loader, { preloader } from "./loader";
import { scopes, tenantId } from "./auth-details";

interface UserDetails {
  id: string;
  name: string;
  role: string;
}

function usersOptions(app: IPublicClientApplication) {
  const load = loader<UserDetails[]>(app, "/api/user?api-version=1.0");

  return queryOptions({
    queryKey: ["users"] as QueryKey,
    queryFn: load,
  });
}

export function useUsers() {
  const msal = useMsal();

  return useSuspenseQuery(usersOptions(msal.instance));
}

export function preloadUsers(
  queryClient: QueryClient,
  app: IPublicClientApplication
) {
  return preloader(queryClient, app, usersOptions(app));
}

export function useUpdateRole() {
  const msal = useMsal();
  const queryClient = useQueryClient();

  return useMutation({
    mutationKey: ["updateRole"],
    mutationFn: async (update: { id: string; role: string }) => {
      const account =
        msal.instance.getActiveAccount() ??
        msal.instance.getAccount({
          tenantId,
        });

      if (account === null) {
        throw new Error("No account available.");
      }

      const tokenRes = await msal.instance.acquireTokenSilent({
        scopes,
        account,
      });

      const authHeader = `Bearer ${tokenRes.accessToken}`;

      const res = await fetch("/api/user?api-version=1.0", {
        method: "POST",
        headers: {
          Authorization: authHeader,
          "Content-Type": "application/json",
        },
        body: JSON.stringify(update),
      });

      if (!res.ok) {
        throw new Error("Failed to update role.");
      }

      queryClient.invalidateQueries({ queryKey: ["users"] });
    },
  });
}
