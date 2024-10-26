// Caution! Be sure you understand the caveats before publishing an application with
// offline support. See https://aka.ms/blazor-offline-considerations

self.importScripts('./service-worker-assets.js');
self.addEventListener('install', event => event.waitUntil(onInstall(event)));
self.addEventListener('activate', event => event.waitUntil(onActivate(event)));
self.addEventListener('fetch', event => event.respondWith(onFetch(event)));

const cacheNamePrefix = 'offline-cache-';
const cacheName = `${cacheNamePrefix}${self.assetsManifest.version}`;
const offlineAssetsInclude = [/\.dll$/, /\.pdb$/, /\.wasm/, /\.html/, /\.js$/, /\.json$/, /\.css$/, /\.woff$/, /\.png$/, /\.jpe?g$/, /\.gif$/, /\.ico$/, /\.blat$/, /\.dat$/, /\.svg$/];
const offlineAssetsExclude = [/^service-worker\.js$/];

async function onInstall(event) {
    console.info('Service worker: Install');

    // Fetch and cache all matching items from the assets manifest
    const assetsRequests = self.assetsManifest.assets
        .filter(asset => offlineAssetsInclude.some(pattern => pattern.test(asset.url)))
        .filter(asset => !offlineAssetsExclude.some(pattern => pattern.test(asset.url)))
        .map(asset => new Request(asset.url, { integrity: asset.hash, cache: 'no-cache' }));
    await caches.open(cacheName).then(cache => cache.addAll(assetsRequests));
}

async function onActivate(event) {
    console.info('Service worker: Activate');

    // Delete unused caches
    const cacheKeys = await caches.keys();
    await Promise.all(cacheKeys
        .filter(key => key.startsWith(cacheNamePrefix) && key !== cacheName)
        .map(key => caches.delete(key)));
}

async function onFetch(event) {
    let cachedResponse = null;
    if (event.request.method === 'GET') {
        // For all navigation requests, try to serve index.html from cache
        const shouldServeIndexHtml = event.request.mode === 'navigate'
            && !event.request.url.includes("/api/") // Exclude API calls, which include the log in process
            && !event.request.url.includes("/.well-known/") // Exclude all .well-known URLs.
            && !event.request.url.includes("/signin-microsoft"); // Exclude Microsoft login process

        const request = shouldServeIndexHtml ? 'index.html' : event.request;
        const cache = await caches.open(cacheName);

        // Check if the request is for /api/user/me
        const isUserMeRequest = event.request.url.includes("/api/user/me");
        // Check if the request is one of the /api/hours or /api/people
        const isDataApi = event.request.url.includes("/api/hours")
            || event.request.url.includes("/api/people")
            || event.request.url.includes("/api/user");

        if (isUserMeRequest) {
            try {
                // Try fetching from the network
                const networkResponse = await fetch(event.request);

                // If network response is okay, cache the result and return it
                if (networkResponse.ok) {
                    await cache.put(event.request, networkResponse.clone());
                    return networkResponse;
                }

                // Only use cache for 5xx server errors
                if (networkResponse.status >= 500 && networkResponse.status < 600) {
                    console.error(`Server error for /api/user/me: ${networkResponse.status}`);
                    cachedResponse = await cache.match(event.request);
                    if (cachedResponse) {
                        return cachedResponse;
                    }
                }

                // Pass through client errors (4xx) and other non-5xx statuses
                return networkResponse;
            } catch (error) {
                // If the system is offline, use the cached value
                console.error(`Network error for /api/user/me: ${error}`);
                cachedResponse = await cache.match(event.request);
                if (cachedResponse) {
                    return cachedResponse;
                }
            }
        }
        else if (isDataApi) {
            // Try to serve from cache first (stale)
            cachedResponse = await cache.match(event.request);

            // Fetch from the network to revalidate the cache
            const networkResponsePromise = fetch(event.request).then(async (networkResponse) => {
                // Check if the data has changed (i.e., not a 304 response)
                if (networkResponse.status === 200) {
                    // Update the cache with the fresh data
                    await cache.put(event.request, networkResponse.clone());

                    // Notify the client that new data is available
                    if (self.clients?.matchAll) {
                        const clients = await self.clients.matchAll();
                        clients.forEach(client => {
                            client.postMessage({
                                type: 'NEW_DATA_AVAILABLE',
                                url: event.request.url
                            });
                        });
                    }
                }
                return networkResponse;
            });

            // Return cached response immediately, and update the cache in the background
            return cachedResponse || networkResponsePromise;
        }
        else {
            // For other GET requests (e.g., index.html or other static resources)
            cachedResponse = await cache.match(request);
        }
    }

    return cachedResponse || fetch(event.request);
}
