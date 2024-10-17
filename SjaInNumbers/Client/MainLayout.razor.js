// Function to preload API endpoints when idle

let dotNetHelper;

function sleep(ms) {
    return new Promise(resolve => setTimeout(resolve, ms));
}

export function setup(helper) {
    dotNetHelper = helper;
}

export function preloadApiEndpoints(endpoints, delay = 500) {
    if ('requestIdleCallback' in window) {
        requestIdleCallback(async () => {
            for (let i = 0; i < endpoints.length; i++) {
                try {
                    // Fetch the current endpoint
                    await fetch(endpoints[i]).then(response => response.json());
                    console.log(`Preloaded: ${endpoints[i]}`);
                } catch (error) {
                    console.error(`Error preloading ${endpoints[i]}:`, error);
                }

                // Wait for the specified delay before the next request
                await sleep(delay);
            }

            dotNetHelper.invokeMethodAsync('OnPreloadComplete');
        });
    } else {
        // Fallback if requestIdleCallback is not supported
        setTimeout(async () => {
            for (let i = 0; i < endpoints.length; i++) {
                try {
                    // Fetch the current endpoint
                    await fetch(endpoints[i]).then(response => response.json());
                    console.log(`Preloaded: ${endpoints[i]}`);
                } catch (error) {
                    console.error(`Error preloading ${endpoints[i]}:`, error);
                }

                // Wait for the specified delay before the next request
                await sleep(delay);
            }

            dotNetHelper.invokeMethodAsync('OnPreloadComplete');
        }, 1000); // A small delay to simulate idle time

    }
}
