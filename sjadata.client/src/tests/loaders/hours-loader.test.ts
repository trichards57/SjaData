import fetchMock from "fetch-mock";
import { beforeEach, describe, expect, it } from "vitest";
import hoursLoader from "../../loaders/hours-loader";

const testData = {
  counts: {
    SW: "1.01:00:00",
    SWAST: "2.00:00:00",
  },
  lastUpdate: "2021-01-02T00:00:00Z",
};

const testParsedData = {
  counts: {
    SW: 25,
    SWAST: 48,
  },
  lastUpdate: new Date("2021-01-02T00:00:00Z"),
};

describe("Hours Loader", () => {
  beforeEach(() => {
    fetchMock.restore();
  });

  it("should return parsed data on successful load", async () => {
    fetchMock.mock(
      { method: "GET", url: "/api/hours/count" },
      {
        body: testData,
      }
    );

    const actual = await hoursLoader();
    expect(actual).toEqual(testParsedData);
  });

  it("should pass date to the API", async () => {
    fetchMock.mock(
      {
        method: "GET",
        url: "/api/hours/count",
        query: { date: "2020-01-01", dateType: "Month" },
      },
      {
        body: testData,
      }
    );

    const date = new Date("2020-01-01");
    const actual = await hoursLoader(date);
    expect(actual).toEqual(testParsedData);
  });

  it("should throw if a bad response is received", () => {
    fetchMock.mock({ method: "GET", url: "/api/hours/count" }, { status: 500 });

    return expect(hoursLoader()).rejects.toThrow(
      "Failed to load hours details."
    );
  });

  it("should throw if request aborts", () => {
    fetchMock.mock(
      { method: "GET", url: "/api/hours/count" },
      { throws: new Error("Failed to fetch") }
    );

    return expect(hoursLoader()).rejects.toThrow("Failed to fetch");
  });
});
