import {
  InteractionRequiredAuthError,
  InteractionType,
  IPublicClientApplication,
  PopupRequest,
  SilentRequest,
} from "@azure/msal-browser";
import { useMsalAuthentication } from "@azure/msal-react";
import { createRootRouteWithContext, Outlet } from "@tanstack/react-router";
import React, { Suspense, useEffect } from "react";
import styles from "./__root.module.css";
import { ReactQueryDevtools } from "@tanstack/react-query-devtools";
import { QueryClient } from "@tanstack/react-query";

const TanStackRouterDevtools =
  process.env.NODE_ENV === "production"
    ? () => null // Render nothing in production
    : React.lazy(() =>
        // Lazy load in development
        import("@tanstack/router-devtools").then((res) => ({
          default: res.TanStackRouterDevtools,
          // For Embedded Mode
          // default: res.TanStackRouterDevtoolsPanel
        }))
      );

export const Route = createRootRouteWithContext<{
  queryClient: QueryClient;
  pca: IPublicClientApplication;
}>()({
  component: function Component() {
    const request: SilentRequest | PopupRequest = {
      scopes: ["User.Read"],
    };

    const { login, error } = useMsalAuthentication(
      InteractionType.Silent,
      request
    );

    useEffect(() => {
      if (error instanceof InteractionRequiredAuthError) {
        login(InteractionType.Redirect, request);
      }
    }, [error]);

    if (error) {
      return (
        <>
          <div className="container">
            <h1>SJA In Numbers</h1>
            <div className={styles["error-message-root"]}>
              <div className={styles["error-message"]}>
                Unable to sign you in. Try refreshing the page.
              </div>
            </div>
          </div>
          <div className="footer-image" />
          {/* <ReactQueryDevtools initialIsOpen={false} />
          <Suspense>
            <TanStackRouterDevtools />
          </Suspense> */}
        </>
      );
    }

    return (
      <>
        <div className="container">
          <h1>SJA In Numbers</h1>
          <Outlet />
        </div>
        <div className="footer-image" />
        {/* <ReactQueryDevtools initialIsOpen={false} />
        <Suspense>
          <TanStackRouterDevtools />
        </Suspense> */}
      </>
    );
  },
});
