import { useMsal } from "@azure/msal-react";
import {
  QueryClient,
  queryOptions,
  useSuspenseQuery,
} from "@tanstack/react-query";
import { Region } from "./hours-loader";
import {
  InteractionRequiredAuthError,
  IPublicClientApplication,
} from "@azure/msal-browser";

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
  const loader = trendsLoader(app);

  return queryOptions({
    queryKey: ["trends", region, nhse],
    queryFn: () => loader(region, nhse),
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

function trendsLoader(app: IPublicClientApplication) {
  return async (region: Region, nhse: boolean = false) => {
    const request = {
      account: app.getAllAccounts()[0],
      scopes: ["User.Read"],
    };

    try {
      const tokenResult = await app.acquireTokenSilent(request);
      const authHeader = `Bearer ${tokenResult.idToken}`;
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
    } catch (error) {
      if (error instanceof InteractionRequiredAuthError) {
        await app.acquireTokenRedirect(request);
      }
      return {
        twelveMonthAverage: {},
        twelveMonthMinusOneAverage: {},
        sixMonthAverage: {},
        sixMonthMinusOneAverage: {},
        threeMonthAverage: {},
        threeMonthMinusOneAverage: {},
        hours: {},
        thresholdDate: "Unknown",
      };
    }
  };
}
