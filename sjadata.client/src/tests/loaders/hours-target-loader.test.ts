import fetchMock from "fetch-mock";
import { beforeEach, describe, expect, it } from "vitest";
import hoursTargetLoader from "../../loaders/hours-target-loader";

describe("Hours Loader", () => {
  beforeEach(() => {
    fetchMock.restore();
  });

  it("should return parsed data on successful load", async () => {
    fetchMock.mock(
      { method: "GET", url: "/api/hours/target" },
      {
        body: {
          target: 20000,
        },
      }
    );

    const actual = await hoursTargetLoader();
    expect(actual).toEqual(20000);
  });

  it("should throw if a bad response is received", () => {
    fetchMock.mock(
      { method: "GET", url: "/api/hours/target" },
      { status: 500 }
    );

    return expect(hoursTargetLoader()).rejects.toThrow(
      "Failed to load hours details."
    );
  });

  it("should throw if request aborts", () => {
    fetchMock.mock(
      { method: "GET", url: "/api/hours/target" },
      { throws: new Error("Failed to fetch") }
    );

    return expect(hoursTargetLoader()).rejects.toThrow("Failed to fetch");
  });
});
