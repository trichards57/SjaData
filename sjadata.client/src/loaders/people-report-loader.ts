import { useMsal } from "@azure/msal-react";
import {
  QueryClient,
  queryOptions,
  useSuspenseQuery,
} from "@tanstack/react-query";
import { Region } from "./hours-loader";
import { IPublicClientApplication } from "@azure/msal-browser";
import loader from "./loader";

export interface PersonReport {
  name: string;
  monthsSinceLastActive: number;
  hours: number[];
  hoursThisYear: number;
}

function peopleReportsOptions(app: IPublicClientApplication, region: Region) {
  const uri = `/api/people/reports?region=${region}&api-version=1.0`;

  const load = loader<PersonReport[]>(app, uri);

  return queryOptions({
    queryKey: ["people", "report", region],
    queryFn: load,
  });
}

export function usePeopleReports(region: Region) {
  const msal = useMsal();

  return useSuspenseQuery(peopleReportsOptions(msal.instance, region));
}

export function preloadPeopleReports(
  queryClient: QueryClient,
  app: IPublicClientApplication,
  region: Region
) {
  queryClient.ensureQueryData(peopleReportsOptions(app, region));
}
