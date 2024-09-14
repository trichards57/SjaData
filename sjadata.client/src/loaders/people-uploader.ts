import { useMsal } from "@azure/msal-react";

export function usePeopleUploader() {
  const msal = useMsal();

  return async (file: File) => {
    const tokenRes = await msal.instance.acquireTokenSilent({
      scopes: ["User.Read"],
      account: msal.instance.getAccount({
        tenantId: "91d037fb-4714-4fe8-b084-68c083b8193f",
      })!,
    });

    const authHeader = `Bearer ${tokenRes.idToken}`;

    const res = await fetch("/api/people?api-version=1.0", {
      method: "POST",
      headers: {
        Authorization: authHeader,
        "Content-Type": "text/csv",
      },
      body: await file.text(),
    });

    return res.ok ? ((await res.json()) as { count: number }) : undefined;
  };
}
