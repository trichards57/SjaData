import { IPublicClientApplication } from "@azure/msal-browser";
import { useMsal } from "@azure/msal-react";
import {
  QueryClient,
  QueryKey,
  queryOptions,
  useSuspenseQuery,
} from "@tanstack/react-query";
import loader, { preloader } from "./loader";

interface UserDetails {
  name: string;
  role: string;
}

function usersOptions(app: IPublicClientApplication) {
  const load = loader<UserDetails>(app, "/api/user?api-version=1.0");

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
