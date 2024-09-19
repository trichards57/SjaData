import { StrictMode } from "react";
import ReactDOM from "react-dom/client";
import { RouterProvider, createRouter } from "@tanstack/react-router";

import "./styles/main.css";

// Import the generated route tree
import { routeTree } from "./routeTree.gen";
import {
  AuthenticationResult,
  Configuration,
  createStandardPublicClientApplication,
  EventType,
} from "@azure/msal-browser";
import { MsalProvider } from "@azure/msal-react";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { clientId, redirectUri, tenantId } from "./loaders/auth-details";

// Register the router instance for type safety
declare module "@tanstack/react-router" {
  interface Register {
    router: typeof router;
  }
}

const configuration: Configuration = {
  auth: {
    clientId,
    authority: `https://login.microsoftonline.com/${tenantId}`,
    redirectUri,
  },
};

const pca = await createStandardPublicClientApplication(configuration);

// Default to using the first account if no account is active on page load
if (!pca.getActiveAccount()) {
  const knownAccount = pca.getAccount({
    tenantId,
  });

  if (knownAccount) {
    console.log("Already logged in - setting account.");
    pca.setActiveAccount(knownAccount);
  }
}

pca.addEventCallback((event) => {
  if (
    event.eventType === EventType.LOGIN_SUCCESS &&
    (event.payload as AuthenticationResult)?.account
  ) {
    const account = (event.payload as AuthenticationResult).account;
    console.log("Logged in - setting account.");
    pca.setActiveAccount(account);
  }
});

try {
  await pca.handleRedirectPromise();
  const account = pca.getActiveAccount();
  if (!account) {
    // redirect anonymous user to login page
    pca.loginRedirect();
  }
} catch (err) {
  // TODO: Handle errors
  console.log(err);
}

const queryClient = new QueryClient({
  defaultOptions: { queries: { staleTime: 30_000 } },
});

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
