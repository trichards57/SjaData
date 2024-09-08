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

export type AreaLabel = Region | Trust;

export interface ParsedHoursCount {
  counts: Partial<Record<AreaLabel, number>>;
  lastUpdate: Date | undefined;
}

export default async function hoursLoader(date?: Date) {
  const uri = date
    ? `/api/hours/count?date=${date.toISOString().split("T")[0]}&dateType=m&api-version=1.0`
    : "/api/hours/count?api-version=1.0";

  const res = await fetch(uri);

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

    const [daysString, time] = value.split(".");
    const parts = time.split(":");
    const days = parseInt(daysString, 10);
    const hours = parseInt(parts[0], 10);
    const minutes = parseInt(parts[1], 10);

    parsedData.counts[key] = days * 24 + hours + minutes / 60;
  }

  return parsedData as Readonly<ParsedHoursCount>;
}
