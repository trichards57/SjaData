import { useMsal } from "@azure/msal-react";
import {
  QueryClient,
  queryOptions,
  useSuspenseQuery,
} from "@tanstack/react-query";
import loader from "./loader";
import { IPublicClientApplication } from "@azure/msal-browser";

interface TargetHours {
  target: number;
}

function hoursTargetOptions(app: IPublicClientApplication, date?: Date) {
  let dateString: string | undefined = undefined;
  if (date) {
    dateString = date.toISOString().split("T")[0];
  }

  const uri = date
    ? `/api/hours/target?date=${dateString}&dateType=m&api-version=1.0`
    : "/api/hours/target?api-version=1.0";

  const load = loader<TargetHours>(app, uri);

  // const loader = hoursTargetLoader(token);

  return queryOptions({
    queryKey: ["hours-target", dateString],
    queryFn: load,
  });
}

export function useHoursTarget(date?: Date) {
  const msal = useMsal();
  return useSuspenseQuery(hoursTargetOptions(msal.instance, date));
}

export function preloadHoursTargetCount(
  queryClient: QueryClient,
  app: IPublicClientApplication,
  date?: Date
) {
  queryClient.ensureQueryData(hoursTargetOptions(app, date));
}
