import { useMsal } from "@azure/msal-react";
import {
  QueryClient,
  queryOptions,
  useSuspenseQuery,
} from "@tanstack/react-query";
import loader from "./loader";
import { IPublicClientApplication } from "@azure/msal-browser";

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

function hoursOptions(
  app: IPublicClientApplication,
  date?: Date,
  future?: boolean
) {
  let dateString: string | undefined = undefined;
  if (date) {
    dateString = date.toISOString().split("T")[0];
  }

  let uri = date
    ? `/api/hours/count?date=${dateString}&dateType=m&api-version=1.0`
    : "/api/hours/count?api-version=1.0";

  if (future) {
    uri = `${uri}&future=true`;
  }

  const load = hoursLoader(app, uri);

  return queryOptions({
    queryKey: ["hours", dateString, future],
    queryFn: load,
  });
}

export function useHoursCount(date?: Date, future: boolean = false) {
  const msal = useMsal();

  return useSuspenseQuery(hoursOptions(msal.instance, date, future));
}

export function preloadHoursCount(
  queryClient: QueryClient,
  app: IPublicClientApplication,
  date?: Date,
  future: boolean = false
) {
  queryClient.ensureQueryData(hoursOptions(app, date, future));
}

function hoursLoader(app: IPublicClientApplication, uri: string) {
  const load = loader<HoursCount>(app, uri);

  return async () => {
    const data = await load();

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
