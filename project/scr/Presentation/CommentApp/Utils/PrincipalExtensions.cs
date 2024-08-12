using IdentityModel;
using Microsoft.AspNetCore.Http;
using Sxb.Web.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Sxb.Web
{
    public static class PrincipalExtensions
    {
        public static Guid GetId(this IIdentity identity)
        {
            var id = identity as ClaimsIdentity;
            var claim = id.FindFirst(JwtClaimTypes.Id);

            if (claim == null) throw new InvalidOperationException("sub claim is missing");

            return  Guid.Parse(claim.Value);
        }

        //public static UserIdentity GetUserInfo(this IIdentity identity)
        //{
        //    var id = identity as ClaimsIdentity;

        //    UserIdentity user = new UserIdentity()
        //    {
        //        UserId = Guid.Parse(id.FindFirst(JwtClaimTypes.Id).Value),
        //        //Name = id.FindFirst(JwtClaimTypes.Name).Value,
        //        //Role = int.Parse( id.FindFirst(JwtClaimTypes.Role).Value),
        //        //Phone = id.FindFirst(JwtClaimTypes.PhoneNumber).Value
        //    };
        //    return user;
        //}


        public static UserIdentity GetUserInfo(this HttpContext context)
        {
            if (context.User.Identity.IsAuthenticated)
            {
                return context.User.Identity.GetUserInfo();
            }
            else
            {
                return null;
            }

        }

        public static UserIdentity GetUserInfo(this IIdentity identity)
        {
            var id = identity as ClaimsIdentity;

            bool IsAuth = false;
            if (id.Claims.Where(x => x.Type == "isAuth").FirstOrDefault() != null)
            {
                IsAuth = bool.Parse(id.Claims.Where(x => x.Type == "isAuth").FirstOrDefault().Value);
            }

            int[] role = new int[0];
            if (id.Claims.Where(x => x.Type == "role").FirstOrDefault() != null)
            {
                role = ChangeRoleArrayInt(id.FindFirst("role").Value);
            }

            UserIdentity user = identity.IsAuthenticated ? new UserIdentity()
            {
                UserId = Guid.Parse(id.FindFirst("id").Value),
                Name = id.FindFirst("name").Value,
                Role = role,
                IsAuth = IsAuth,
                //Phone = id.FindFirst(JwtClaimTypes.PhoneNumber).Value
            } : new UserIdentity();
            return user;
        }

        private static int[] ChangeRoleArrayInt(string roleString)
        {
            string[] stringArray = (roleString ?? "").Split(',');

            List<int> result = new List<int>();

            foreach (var item in stringArray.Where(q => !string.IsNullOrWhiteSpace(q)))
            {
                result.Add(int.Parse(item));
            }
            return result.ToArray();
        }
    }
}
