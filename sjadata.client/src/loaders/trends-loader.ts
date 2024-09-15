import { useMsal } from "@azure/msal-react";
import {
  QueryClient,
  queryOptions,
  useSuspenseQuery,
} from "@tanstack/react-query";
import { Region } from "./hours-loader";

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

function trendsOptions(token: string, region: Region, nhse: boolean = false) {
  const loader = trendsLoader(token);

  return queryOptions({
    queryKey: ["trends", region, nhse],
    queryFn: () => loader(region, nhse),
  });
}

export function useTrends(region: Region, nhse: boolean = false) {
  const msal = useMsal();

  return useSuspenseQuery(
    trendsOptions(msal.accounts[0].idToken ?? "", region, nhse)
  );
}

export function preloadTrends(
  queryClient: QueryClient,
  token: string,
  region: Region,
  nhse: boolean = false
) {
  queryClient.ensureQueryData(trendsOptions(token, region, nhse));
}

function trendsLoader(token: string) {
  const authHeader = `Bearer ${token}`;

  return async (region: Region, nhse: boolean = false) => {
    let uri = `/api/hours/trends?region=${region}&api-version=1.0`;

    if (nhse) {
      uri += "&nhse=true";
    }

    const res = await fetch(uri, {
      headers: {
        Authorization: authHeader,
      },
    });

    if (!res.ok) throw new Error("Failed to load hours trends.");

    const data = (await res.json()) as Trends;

    return data as Readonly<Trends>;
  };
}
