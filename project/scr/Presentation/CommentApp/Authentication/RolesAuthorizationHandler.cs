using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace Sxb.Web.Authentication
{
    public class RolesAuthorizationHandler : AuthorizationHandler<RolesAuthorizationRequirement>,IAuthorizationHandler
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
            RolesAuthorizationRequirement requirement)
        {
            if (context.User == null || requirement == null)
            {
                return Task.FromResult(0);
            }

            if (requirement.AllowedRoles.Any(role => context.User.HasClaim(q=>q.Value.Split(',').Contains(role))))
            {
                context.Succeed(requirement);
            }

            return Task.FromResult(0);
        }
    }
}
