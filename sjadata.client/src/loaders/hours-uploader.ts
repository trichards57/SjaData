import { useMsal } from "@azure/msal-react";

export function useHoursUploader() {
  const msal = useMsal();

  return async (file: File) => {
    const tokenRes = await msal.instance.acquireTokenSilent({
      scopes: ["User.Read"],
      account: msal.instance.getAccount({
        tenantId: "91d037fb-4714-4fe8-b084-68c083b8193f",
      })!,
    });

    const authHeader = `Bearer ${tokenRes.idToken}`;

    const formData = new FormData();
    formData.append("file", file);

    const res = await fetch("/api/hours?api-version=1.0", {
      method: "POST",
      headers: {
        Authorization: authHeader,
      },
      body: formData,
    });

    return res.ok ? ((await res.json()) as { count: number }) : undefined;
  };
}
