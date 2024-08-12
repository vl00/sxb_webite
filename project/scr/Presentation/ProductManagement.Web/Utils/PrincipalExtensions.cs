using IdentityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace ProductManagement.Web
{
    public static class PrincipalExtensions
    {
        public static string GetId(this IIdentity identity)
        {
            var id = identity as ClaimsIdentity;
            var claim = id.FindFirst(JwtClaimTypes.Id);

            if (claim == null) throw new InvalidOperationException("sub claim is missing");

            return  claim.Value;
        }
    }
}
