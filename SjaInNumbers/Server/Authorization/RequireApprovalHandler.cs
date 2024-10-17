using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using SjaInNumbers.Server.Data;

namespace SjaInNumbers.Server.Authorization;

public class RequireApprovalHandler : AuthorizationHandler<RequireApprovalRequirement>
{
    private readonly UserManager<ApplicationUser> userManager;

    public RequireApprovalHandler(UserManager<ApplicationUser> userManager)
    {
        this.userManager = userManager;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, RequireApprovalRequirement requirement)
    {
        var user = await userManager.GetUserAsync(context.User);

        if (user?.IsApproved == true)
        {
            context.Succeed(requirement);
        }
    }
}
