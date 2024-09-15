import { useMsal } from "@azure/msal-react";
import {
  QueryClient,
  queryOptions,
  useSuspenseQuery,
} from "@tanstack/react-query";

interface HoursCount {
  counts: Partial<Record<AreaLabel, string>>;
  lastUpdate: string;
}

export type Trust =
  | "NEAS"
  | "NWAS"
  | "WMAS"
  | "EMAS"
  | "EEAST"
  | "LAS"
  | "SECAMB"
  | "SWAST"
  | "SCAS"
  | "YAS"
  | "WAST"
  | "SAS"
  | "NIAS"
  | "IWAS";

export type Region = "NE" | "NW" | "WM" | "EM" | "EOE" | "LON" | "SE" | "SW";

export const Regions: Region[] = [
  "NE",
  "NW",
  "WM",
  "EM",
  "EOE",
  "LON",
  "SE",
  "SW",
];

export function regionToString(region: Region) {
  switch (region) {
    case "NE":
      return "North East";
    case "NW":
      return "North West";
    case "WM":
      return "West Midlands";
    case "EM":
      return "East Midlands";
    case "EOE":
      return "East of England";
    case "LON":
      return "London";
    case "SE":
      return "South East";
    case "SW":
      return "South West";
  }
}

export type AreaLabel = Region | Trust;

export interface ParsedHoursCount {
  counts: Partial<Record<AreaLabel, number>>;
  lastUpdate: Date | undefined;
}

function hoursOptions(token: string, date?: Date, future?: boolean) {
  let dateString: string | undefined = undefined;
  if (date) {
    dateString = date.toISOString().split("T")[0];
  }

  const loader = hoursLoader(token);

  return queryOptions({
    queryKey: ["hours", dateString, future],
    queryFn: () => loader(dateString, future),
  });
}

export function useHoursCount(date?: Date, future: boolean = false) {
  const msal = useMsal();

  return useSuspenseQuery(
    hoursOptions(msal.accounts[0].idToken ?? "", date, future)
  );
}

export function preloadHoursCount(
  queryClient: QueryClient,
  token: string,
  date?: Date,
  future: boolean = false
) {
  queryClient.ensureQueryData(hoursOptions(token, date, future));
}

function hoursLoader(token: string) {
  const authHeader = `Bearer ${token}`;

  return async (date?: string, future: boolean = false) => {
    let uri = date
      ? `/api/hours/count?date=${date}&dateType=m&api-version=1.0`
      : "/api/hours/count?api-version=1.0";

    if (future) {
      uri = `${uri}&future=true`;
    }

    const res = await fetch(uri, {
      headers: {
        Authorization: authHeader,
      },
    });

    if (!res.ok) throw new Error("Failed to load hours details.");

    const data = (await res.json()) as HoursCount;

    const parsedData: ParsedHoursCount = {
      counts: {},
      lastUpdate:
        data.lastUpdate === "0001-01-01T00:00:00+00:00"
          ? undefined
          : new Date(data.lastUpdate),
    };

    for (const key of Object.keys(data.counts) as AreaLabel[]) {
      const value = data.counts[key];

      if (!value) {
        continue;
      }

      let daysString: string = "0";
      let time: string = "";

      if (value.includes(".")) {
        [daysString, time] = value.split(".");
      } else {
        time = value;
      }

      const parts = time.split(":");
      const days = parseInt(daysString, 10);
      const hours = parseInt(parts[0], 10);
      const minutes = parseInt(parts[1], 10);

      parsedData.counts[key] = days * 24 + hours + minutes / 60;
    }

    return parsedData as Readonly<ParsedHoursCount>;
  };
}
