import { createFileRoute, notFound } from "@tanstack/react-router";
import { Region, Regions } from "../loaders/hours-loader";
import { preloadPeopleReports, usePeopleReports } from "../loaders/people-report-loader";

interface PeopleReportsPageProps {
  region: Region;
}

export function PeopleReportsPage({ region }: PeopleReportsPageProps) {
  const { data: reports } = usePeopleReports(region);

  return <div>Ji</div>;
}

export const Route = createFileRoute("/people/$region")({
  beforeLoad: async (ctx) => {
    const { region } = ctx.params;
    if (!Regions.includes(region as Region)) {
      throw notFound();
    }
  },
  loader: async ({ context, params }) => {
    const tokenRes = await context.pca.acquireTokenSilent({
      scopes: ["User.Read"],
      account: context.pca.getAccount({
        tenantId: "91d037fb-4714-4fe8-b084-68c083b8193f",
      })!,
    });
    const token = tokenRes.idToken;

    return preloadPeopleReports(
      context.queryClient,
      token,
      params.region as Region
    );
  },
  component: function Component() {
    const { region } = Route.useParams();
    return <PeopleReportsPage region={region as Region} />;
  },
});
