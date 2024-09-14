import { StrictMode } from "react";
import ReactDOM from "react-dom/client";
import { RouterProvider, createRouter } from "@tanstack/react-router";

import "./styles/main.css";

// Import the generated route tree
import { routeTree } from "./routeTree.gen";
import { Configuration, PublicClientApplication } from "@azure/msal-browser";
import { MsalProvider } from "@azure/msal-react";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";

// Register the router instance for type safety
declare module "@tanstack/react-router" {
  interface Register {
    router: typeof router;
  }
}

const configuration: Configuration = {
  auth: {
    clientId: "a984d5ce-d914-47d0-b690-1bcf084eb829",
    authority:
      "https://login.microsoftonline.com/91d037fb-4714-4fe8-b084-68c083b8193f",
    redirectUri: "https://localhost:5173/signin",
  },
};

const pca = new PublicClientApplication(configuration);
await pca.initialize();

const queryClient = new QueryClient();

// Create a new router instance
const router = createRouter({
  routeTree,
  defaultPreload: "intent",
  context: { queryClient, pca },
  defaultPreloadStaleTime: 0,
});

// Render the app
const rootElement = document.getElementById("root")!;
if (!rootElement.innerHTML) {
  const root = ReactDOM.createRoot(rootElement);
  root.render(
    <StrictMode>
      <MsalProvider instance={pca}>
        <QueryClientProvider client={queryClient}>
          <RouterProvider router={router} context={{ queryClient, pca }} />
        </QueryClientProvider>
      </MsalProvider>
    </StrictMode>
  );
}
