export const tenantId = "91d037fb-4714-4fe8-b084-68c083b8193f";
export const clientId =
  process.env.NODE_ENV === "production"
    ? "984845d8-6b16-4ebf-97d4-925b25bf5f1e"
    : "d4237883-352f-414a-b6cd-f9afe2beb7db";
export const redirectUri = "https://localhost:5173";

export const scopes = ["email", "profile", `${clientId}/.default`];
