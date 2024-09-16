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
  SilentRequest,
} from "@azure/msal-browser";

export interface PersonReport {
  name: string;
  monthsSinceLastActive: number;
  hours: number[];
  hoursThisYear: number;
}

function peopleReportsOptions(app: IPublicClientApplication, region: Region) {
  const loader = peopleReportsLoader(app);

  return queryOptions({
    queryKey: ["people", "report", region],
    queryFn: () => loader(region),
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

function peopleReportsLoader(app: IPublicClientApplication) {
  return async (region: Region) => {
    const request: SilentRequest = {
      account: app.getAllAccounts()[0],
      scopes: ["User.Read"],
      forceRefresh: true,
    };

    try {
      const tokenResult = await app.acquireTokenSilent(request);
      const authHeader = `Bearer ${tokenResult.idToken}`;
      const uri = `/api/people/reports?region=${region}&api-version=1.0`;

      const res = await fetch(uri, {
        headers: {
          Authorization: authHeader,
        },
      });

      if (!res.ok) throw new Error("Failed to load hours trends.");

      return (await res.json()) as PersonReport[];
    } catch (error) {
      if (error instanceof InteractionRequiredAuthError) {
        await app.acquireTokenRedirect(request);
      }
      return [];
    }
  };
}
