using Microsoft.AspNetCore.Authorization;

namespace SJAData.Authorization;

public class RequireApprovalFailureMiddleware
{
    private readonly RequestDelegate next;

    public RequireApprovalFailureMiddleware(RequestDelegate next)
    {
        this.next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Capture the original response
        var originalResponse = context.Response;

        // Create a memory stream to capture the response
        using var newResponseStream = new MemoryStream();

        // Swap the response body with our stream
        var originalResponseBody = originalResponse.Body;
        context.Response.Body = newResponseStream;

        // Call the next middleware
        await next(context);

        // Check for a 403 Forbidden status code
        if (context.Response.StatusCode == StatusCodes.Status403Forbidden)
        {
            // Check if the failed policy is the specific one you're interested in
            var endpoint = context.GetEndpoint();
            if (endpoint != null)
            {
                var authMetadata = endpoint.Metadata.GetMetadata<AuthorizeAttribute>();
                if (authMetadata != null && authMetadata.Policy == "Approved")
                {
                    // Redirect if the specific policy failed
                    context.Response.Redirect("/Account/ApprovalNeeded");
                }
            }
        }

        // Copy the contents of the new response stream back to the original stream
        newResponseStream.Seek(0, SeekOrigin.Begin);
        await newResponseStream.CopyToAsync(originalResponseBody);
        context.Response.Body = originalResponseBody;
    }
}
