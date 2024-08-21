interface TargetHours {
  target: number;
}

export default async function hoursTargetLoader(date?: Date) {
  const uri = date
    ? `/api/hours/target?date=${date.toDateString()}&dateType=Month`
    : "/api/hours/target";

  const res = await fetch(uri);

  if (!res.ok) throw new Error("Failed to load hours details.");

  const data = (await res.json()) as TargetHours;

  return data.target;
}
