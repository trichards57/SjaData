import {
  InteractionRequiredAuthError,
  IPublicClientApplication,
} from "@azure/msal-browser";
import { useMsal } from "@azure/msal-react";
import {
  QueryClient,
  queryOptions,
  useSuspenseQuery,
} from "@tanstack/react-query";

interface UserDetails {
  name: string;
  role: string;
}

function meOptions(app: IPublicClientApplication) {
  const loader = meLoader(app);

  return queryOptions({
    queryKey: ["user", "me"],
    queryFn: loader,
  });
}

export function useMe() {
  const msal = useMsal();

  return useSuspenseQuery(meOptions(msal.instance));
}

export function preloadMe(
  queryClient: QueryClient,
  app: IPublicClientApplication
) {
  queryClient.ensureQueryData(meOptions(app));
}

function meLoader(app: IPublicClientApplication) {
  return async () => {
    const request = {
      account: app.getAllAccounts()[0],
      scopes: ["User.Read"],
    };

    try {
      const tokenResult = await app.acquireTokenSilent(request);
      const authHeader = `Bearer ${tokenResult.idToken}`;
      const uri = "/api/user/me?api-version=1.0";

      const res = await fetch(uri, {
        headers: {
          Authorization: authHeader,
        },
      });

      if (!res.ok) throw new Error("Failed to load my user details.");

      const data = (await res.json()) as UserDetails;

      return data as Readonly<UserDetails>;
    } catch (error) {
      if (error instanceof InteractionRequiredAuthError) {
        await app.acquireTokenRedirect(request);
      }
      return { name: "Anonymous", role: "guest" };
    }
  };
}
