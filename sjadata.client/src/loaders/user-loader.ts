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

function meOptions(app: IPublicClientApplication) {
  const load = loader<UserDetails>(app, "/api/user/me?api-version=1.0");

  return queryOptions({
    queryKey: ["user", "me"] as QueryKey,
    queryFn: load,
  });
}

export function useMe() {
  const msal = useMsal();

  return useSuspenseQuery(meOptions(msal.instance));
}

export function preloadMe(
  queryClient: QueryClient,
  app: IPublicClientApplication
) {
  return preloader(queryClient, app, meOptions(app));
}
