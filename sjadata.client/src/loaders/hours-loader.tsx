interface HoursCount {
  counts: Record<string, string>;
  lastUpdate: string;
}

export interface ParsedHoursCount {
  counts: Record<string, number>;
  lastUpdate: Date;
}

export default async function hoursLoader(date?: Date) {
  const uri = date
    ? `/api/hours/count?date=${date.toISOString().split("T")[0]}&dateType=Month`
    : "/api/hours/count";

  const res = await fetch(uri);

  if (!res.ok) throw new Error("Failed to load hours details.");

  const data = (await res.json()) as HoursCount;

  const parsedData: ParsedHoursCount = {
    counts: {},
    lastUpdate: new Date(data.lastUpdate),
  };

  for (const [key, value] of Object.entries(data.counts)) {
    const [daysString, time] = value.split(".");
    const parts = time.split(":");
    const days = parseInt(daysString, 10);
    const hours = parseInt(parts[0], 10);
    const minutes = parseInt(parts[1], 10);

    parsedData.counts[key] = Math.round(days * 24 + hours + minutes / 60);
  }

  return parsedData as Readonly<ParsedHoursCount>;
}
