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

function meOptions(token: string) {
  const loader = meLoader(token);

  return queryOptions({
    queryKey: ["user", "me"],
    queryFn: () => loader(),
  });
}

export function useMe() {
  const msal = useMsal();

  return useSuspenseQuery(meOptions(msal.accounts[0].idToken ?? ""));
}

export function preloadMe(queryClient: QueryClient, token: string) {
  queryClient.ensureQueryData(meOptions(token));
}

function meLoader(token: string) {
  const authHeader = `Bearer ${token}`;

  return async () => {
    const uri = "/api/user/me?api-version=1.0";

    const res = await fetch(uri, {
      headers: {
        Authorization: authHeader,
      },
    });

    if (!res.ok) throw new Error("Failed to load my user details.");

    const data = (await res.json()) as UserDetails;

    return data as Readonly<UserDetails>;
  };
}
