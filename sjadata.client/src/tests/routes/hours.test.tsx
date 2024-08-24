import { describe, expect, it } from "vitest";
import { render } from "@testing-library/react";
import { Hours } from "../../routes/hours";
import { ParsedHoursCount } from "../../loaders/hours-loader";

const testMonthData: ParsedHoursCount = {
  counts: {
    NE: 11.3,
    NW: 12.3,
  },
  lastUpdate: new Date("2022-01-02T00:00:00Z"),
};

const testYearData: ParsedHoursCount = {
  counts: {
    NE: 153,
    NW: 143,
  },
  lastUpdate: new Date("2022-01-02T00:00:00Z"),
};

const testTarget = 2000;

describe("Hours Page", () => {
  it("renders data correctly", async () => {
    const { asFragment } = render(
      <Hours month={testMonthData} ytd={testYearData} target={testTarget} />
    );

    expect(asFragment()).toMatchSnapshot();
  });
});
