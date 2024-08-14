import { createFileRoute } from "@tanstack/react-router";

export const Route = createFileRoute("/hours")({
  component: Hours,
});

function Hours() {
  return (
    <>
      <h2>Hours</h2>
      <div className="link-boxes">
        <div className="hours-box month">
          <div>This Month</div>
          <div>0</div>
        </div>
        <div className="hours-box ytd">
          <div>Year to Date</div>
          <div>0</div>
        </div>
        <div className="hours-box target">
          <div>NHSE Target</div>
          <div>0</div>
        </div>
      </div>
    </>
  );
}
