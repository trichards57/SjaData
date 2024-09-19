import { useMsal } from "@azure/msal-react";
import {
  QueryClient,
           QueryKey,
  queryOptions,
  useSuspenseQuery,
} from "@tanstack/react-query";
import loader, { preloader } from "./loader";
import { IPublicClientApplication } from "@azure/msal-browser";
import { addMonths, formatISO } from "date-fns";

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
  queryTemplate: string,
  key: string
) {
  const dateString = formatISO(new Date(), { representation: "date" });

  const uri = `/api/hours/count?date=${dateString}${queryTemplate}&api-version=1.0`;

  const load = hoursLoader(app, uri);

  return queryOptions({
    queryKey: ["hours", dateString, key] as QueryKey,
    queryFn: load,
  });
}

function futureHoursOptions(app: IPublicClientApplication) {
  return hoursOptions(app, "&date-type=m&future=true", "future");
}

function yearToDateOptions(app: IPublicClientApplication) {
  return hoursOptions(app, "&date-type=y", "ytd");
}

function monthToDateOptions(app: IPublicClientApplication) {
  return hoursOptions(app, "&date-type=m", "mtd");
}

function lastMonthOptions(app: IPublicClientApplication) {
  const dateString = formatISO(addMonths(new Date(), -1), {
    representation: "date",
  });

  const uri = `/api/hours/count?date=${dateString}&date-type=m&future=true&api-version=1.0`;

  const load = hoursLoader(app, uri);

  return queryOptions({
    queryKey: ["hours", dateString] as QueryKey,
    queryFn: load,
  });
}

export function useFutureHoursCount() {
  const msal = useMsal();

  return useSuspenseQuery(futureHoursOptions(msal.instance));
}

export function useYearToDateHoursCount() {
  const msal = useMsal();

  return useSuspenseQuery(yearToDateOptions(msal.instance));
}

export function useMonthToDateHoursCount() {
  const msal = useMsal();

  return useSuspenseQuery(monthToDateOptions(msal.instance));
}

export function useLastMonthHoursCount() {
  const msal = useMsal();

  return useSuspenseQuery(lastMonthOptions(msal.instance));
}

export function preloadFutureHoursCount(
  queryClient: QueryClient,
  app: IPublicClientApplication
) {
  return preloader(queryClient, app, futureHoursOptions(app));
}

export function preloadYearToDateHoursCount(
  queryClient: QueryClient,
  app: IPublicClientApplication
) {
  return preloader(queryClient, app, yearToDateOptions(app));
}

export function preloadMonthToDateHoursCount(
  queryClient: QueryClient,
  app: IPublicClientApplication
) {
  return preloader(queryClient, app, monthToDateOptions(app));
}

export function preloadLastMonthHoursCount(
  queryClient: QueryClient,
  app: IPublicClientApplication
) {
  return preloader(queryClient, app, lastMonthOptions(app));
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
