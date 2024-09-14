import { useMsal } from "@azure/msal-react";
import {
  QueryClient,
  queryOptions,
  useSuspenseQuery,
} from "@tanstack/react-query";

interface TargetHours {
  target: number;
}

function hoursTargetOptions(token: string, date?: Date) {
  let dateString: string | undefined = undefined;
  if (date) {
    dateString = date.toISOString().split("T")[0];
  }

  const loader = hoursTargetLoader(token);

  return queryOptions({
    queryKey: ["hours-target", dateString],
    queryFn: () => loader(dateString),
  });
}

export function useHoursTarget(date?: Date) {
  const msal = useMsal();
  return useSuspenseQuery(
    hoursTargetOptions(msal.accounts[0].idToken ?? "", date)
  );
}

export function preloadHoursTargetCount(
  queryClient: QueryClient,
  token: string,
  date?: Date
) {
  queryClient.ensureQueryData(hoursTargetOptions(token, date));
}

function hoursTargetLoader(token: string) {
  const authHeader = `Bearer ${token}`;
  console.log(authHeader);

  return async (date?: string) => {
    const uri = date
      ? `/api/hours/target?date=${date}&dateType=m&api-version=1.0`
      : "/api/hours/target?api-version=1.0";

    const res = await fetch(uri, {
      headers: {
        Authorization: authHeader,
      },
    });

    if (!res.ok) throw new Error("Failed to load hours details.");

    const data = (await res.json()) as TargetHours;

    return data.target;
  };
}
