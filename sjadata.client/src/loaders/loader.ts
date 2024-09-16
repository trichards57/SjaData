import {
  InteractionRequiredAuthError,
  IPublicClientApplication,
} from "@azure/msal-browser";

export default function loader<T>(app: IPublicClientApplication, uri: string) {
  return async () => {
    const account =
      app.getActiveAccount() ??
      app.getAccount({
        tenantId: "91d037fb-4714-4fe8-b084-68c083b8193f",
      });

    if (account === null) {
      throw new Error("No account available.");
    }

    const request = {
      account,
      scopes: ["User.Read"],
    };

    try {
      const tokenResult = await app.acquireTokenSilent(request);
      const authHeader = `Bearer ${tokenResult.idToken}`;

      const res = await fetch(uri, {
        headers: {
          Authorization: authHeader,
        },
      });

      if (!res.ok) throw new Error("Failed to load.");

      const data = (await res.json()) as T;

      return data as Readonly<T>;
    } catch (error) {
      if (error instanceof InteractionRequiredAuthError) {
        await app.acquireTokenRedirect(request);
      }
      throw error;
    }
  };
}
