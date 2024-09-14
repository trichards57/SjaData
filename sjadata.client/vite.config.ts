/// <reference types="vitest" />
import { fileURLToPath, URL } from "node:url";
import { patchCssModules } from "vite-css-modules";

import { defineConfig } from "vite";
import plugin from "@vitejs/plugin-react";
import { TanStackRouterVite } from "@tanstack/router-plugin/vite";
import fs from "fs";
import path from "path";
import child_process from "child_process";
import { env } from "process";

const baseFolder =
  env.APPDATA !== undefined && env.APPDATA !== ""
    ? `${env.APPDATA}/ASP.NET/https`
    : `${env.HOME}/.aspnet/https`;

const certificateName = "sjadata.client";
const certFilePath = path.join(baseFolder, `${certificateName}.pem`);
const keyFilePath = path.join(baseFolder, `${certificateName}.key`);

if (!fs.existsSync(certFilePath) || !fs.existsSync(keyFilePath)) {
  if (
    0 !==
    child_process.spawnSync(
      "dotnet",
      [
        "dev-certs",
        "https",
        "--export-path",
        certFilePath,
        "--format",
        "Pem",
        "--no-password",
      ],
      { stdio: "inherit" }
    ).status
  ) {
    throw new Error("Could not create certificate.");
  }
}

const target = env.ASPNETCORE_HTTPS_PORT
  ? `https://localhost:${env.ASPNETCORE_HTTPS_PORT}`
  : env.ASPNETCORE_URLS
    ? env.ASPNETCORE_URLS.split(";")[0]
    : "https://localhost:7125";

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [patchCssModules(), TanStackRouterVite(), plugin()],
  resolve: {
    alias: {
      "@": fileURLToPath(new URL("./src", import.meta.url)),
    },
  },
  server: {
    proxy: {
      "^/img": {
        target,
        secure: false,
      },
      "^/api": {
        target,
        secure: false,
        configure: (proxy) => {
          proxy.on("proxyReq", (req) => {
            if (req.getHeaders()["content-length"]) {
              const contentLength = parseInt(
                req.getHeaders()["content-length"]?.toString() ?? "0"
              );
              if (contentLength > 100_000_000) {
                // Example: 100MB limit
                req.setHeader("content-length", contentLength);
              }
            }
          });
        },
      },
    },
    port: 5173,
    https: {
      key: fs.readFileSync(keyFilePath),
      cert: fs.readFileSync(certFilePath),
    },
  },
  test: {
    environment: "jsdom",
    setupFiles: ["./setupVitest.ts"],
  },
});
