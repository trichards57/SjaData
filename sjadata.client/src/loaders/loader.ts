import {
  InteractionRequiredAuthError,
  IPublicClientApplication,
} from "@azure/msal-browser";
import { scopes, tenantId } from "./auth-details";

export default function loader<T>(app: IPublicClientApplication, uri: string) {
  return async () => {
    const account =
      app.getActiveAccount() ??
      app.getAccount({
        tenantId,
      });

    if (account === null) {
      throw new Error("No account available.");
    }

    const request = {
      account,
      scopes,
    };

    try {
      const tokenResult = await app.acquireTokenSilent(request);
      const authHeader = `Bearer ${tokenResult.accessToken}`;

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
