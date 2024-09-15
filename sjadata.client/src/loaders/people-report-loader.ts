import { useMsal } from "@azure/msal-react";
import {
  QueryClient,
  queryOptions,
  useSuspenseQuery,
} from "@tanstack/react-query";
import { Region } from "./hours-loader";

export interface PersonReport {
  name: string;
  monthsSinceLastActive: number;
  hours: number[];
  hoursThisYear: number;
}

function peopleReportsOptions(token: string, region: Region) {
  const loader = peopleReportsLoader(token);

  return queryOptions({
    queryKey: ["people", "report", region],
    queryFn: () => loader(region),
  });
}

export function usePeopleReports(region: Region) {
  const msal = useMsal();

  return useSuspenseQuery(
    peopleReportsOptions(msal.accounts[0].idToken ?? "", region)
  );
}

export function preloadPeopleReports(
  queryClient: QueryClient,
  token: string,
  region: Region
) {
  queryClient.ensureQueryData(peopleReportsOptions(token, region));
}

function peopleReportsLoader(token: string) {
  const authHeader = `Bearer ${token}`;

  return async (region: Region) => {
    const uri = `/api/people/reports?region=${region}&api-version=1.0`;

    const res = await fetch(uri, {
      headers: {
        Authorization: authHeader,
      },
    });

    if (!res.ok) throw new Error("Failed to load hours trends.");

    return (await res.json()) as PersonReport[];
  };
}
