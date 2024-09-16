import { useMsal } from "@azure/msal-react";
import {
  QueryClient,
  queryOptions,
  useSuspenseQuery,
} from "@tanstack/react-query";
import { Region } from "./hours-loader";
import { IPublicClientApplication } from "@azure/msal-browser";
import loader from "./loader";

export interface Trends {
  twelveMonthAverage: Record<string, number>;
  twelveMonthMinusOneAverage: Record<string, number>;
  sixMonthAverage: Record<string, number>;
  sixMonthMinusOneAverage: Record<string, number>;
  threeMonthAverage: Record<string, number>;
  threeMonthMinusOneAverage: Record<string, number>;
  hours: Record<string, number[]>;
  thresholdDate: string;
}

function trendsOptions(
  app: IPublicClientApplication,
  region: Region,
  nhse: boolean = false
) {
  let uri = `/api/hours/trends?region=${region}&api-version=1.0`;

  if (nhse) {
    uri += "&nhse=true";
  }

  const load = loader<Trends>(app, uri);

  return queryOptions({
    queryKey: ["trends", region, nhse],
    queryFn: load,
  });
}

export function useTrends(region: Region, nhse: boolean = false) {
  const msal = useMsal();

  return useSuspenseQuery(trendsOptions(msal.instance, region, nhse));
}

export function preloadTrends(
  queryClient: QueryClient,
  app: IPublicClientApplication,
  region: Region,
  nhse: boolean = false
) {
  queryClient.ensureQueryData(trendsOptions(app, region, nhse));
}
