import {
  IPublicClientApplication,
  PopupRequest,
  SilentRequest,
} from "@azure/msal-browser";
import { createRootRouteWithContext, Outlet } from "@tanstack/react-router";
import React, { Suspense } from "react";
import styles from "./__root.module.css";
import { ReactQueryDevtools } from "@tanstack/react-query-devtools";
import { QueryClient } from "@tanstack/react-query";
import { scopes } from "../loaders/auth-details";
import { Loading } from "../components/loading";

const TanStackRouterDevtools =
  process.env.NODE_ENV === "production"
    ? () => null
    : React.lazy(() =>
        import("@tanstack/router-devtools").then((res) => ({
          default: res.TanStackRouterDevtools,
        }))
      );

const request: SilentRequest | PopupRequest = {
  scopes,
};

export const Route = createRootRouteWithContext<{
  queryClient: QueryClient;
  pca: IPublicClientApplication;
}>()({
  pendingComponent: Loading,
  // loader: async ({ context }) => {
  //   const { queryClient, pca } = context;

  //   try {
  //     const authResult = await pca.acquireTokenSilent(request);
  //     return { queryClient, pca, authResult };
  //   } catch {
  //     await pca.acquireTokenRedirect(request);
  //   }
  // },
  component: function Component() {
    // const data = Route.useLoaderData();

    // if (!data?.authResult) {
    //   console.log(data);

    //   return (
    //     <>
    //       <div className="container">
    //         <h1>SJA In Numbers</h1>
    //         <div className={styles["error-message-root"]}>
    //           <div className={styles["error-message"]}>
    //             Unable to sign you in. Try refreshing the page.
    //           </div>
    //         </div>
    //       </div>
    //       <div className="footer-image" />
    //       <ReactQueryDevtools initialIsOpen={false} />
    //       <Suspense>
    //         <TanStackRouterDevtools />
    //       </Suspense>
    //     </>
    //   );
    // }

    return (
      <>
        <div className="container">
          <h1>SJA In Numbers</h1>
          <Outlet />
        </div>
        <div className="footer-image" />
        <ReactQueryDevtools initialIsOpen={false} />
        <Suspense>
          <TanStackRouterDevtools />
        </Suspense>
      </>
    );
  },
});
