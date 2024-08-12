using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using PMS.UserManage.Domain.Entities;
using ProductManagement.Framework.Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace Sxb.UserCenter.Utils
{
    public class JwtCookieHelper
    {
        public static async Task SetSignInCookie(HttpContext httpContext , UserInfo userInfo, TalentEntity talent)
        {
            if (!string.IsNullOrEmpty(userInfo.Mobile))
            {
                userInfo.Mobile = CommonHelper.HideNumber(userInfo.Mobile);
            }

            bool isAuth = false;
            if (talent != null)
            {
                isAuth = talent.status == 1 && talent.certification_status == 1;
            }


            var claims = new List<Claim>() {
                new Claim(JwtClaimTypes.Name, userInfo.NickName),
                new Claim(JwtClaimTypes.Id, userInfo.Id.ToString()),
                new Claim(JwtClaimTypes.PhoneNumber, userInfo.Mobile??""),
                new Claim(JwtClaimTypes.Picture, userInfo.HeadImgUrl??""),
                new Claim(JwtClaimTypes.AccessTokenHash, Guid.NewGuid().ToString()),
                new Claim("isAuth",isAuth.ToString())
            };
            //List<byte> role = new List<byte>();
            //foreach(var verify in verifies)
            //{
            //    if (verify.valid)
            //    {
            //        role.Add(verify.verifyType);
            //    }
            //}
            if (talent != null)
            {
                claims.Add(new Claim(JwtClaimTypes.Role, talent.type.ToString()));
            }

            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme, JwtClaimTypes.Name, JwtClaimTypes.Id);
            identity.AddClaims(claims);
            var princial = new ClaimsPrincipal(identity);
            var properties = new AuthenticationProperties();
            var ticket = new AuthenticationTicket(princial, properties, "CookieScheme");
            await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, princial);
        }

        public static string GetSignInToken(UserInfo userInfo, TalentEntity talent)
        {
            if (!string.IsNullOrEmpty(userInfo.Mobile))
            {
                userInfo.Mobile = CommonHelper.HideNumber(userInfo.Mobile);
            }

            bool isAuth = false;
            if (talent != null)
            {
                isAuth = talent.status == 1 && talent.certification_status == 1;
            }


            var claims = new List<Claim>() {
                new Claim(JwtClaimTypes.Name, userInfo.NickName),
                new Claim(JwtClaimTypes.Id, userInfo.Id.ToString()),
                new Claim(JwtClaimTypes.PhoneNumber, userInfo.Mobile??""),
                new Claim(JwtClaimTypes.Picture, userInfo.HeadImgUrl??""),
                new Claim(JwtClaimTypes.AccessTokenHash, Guid.NewGuid().ToString()),
                new Claim("isAuth",isAuth.ToString())
            };
            if (talent != null)
            {
                claims.Add(new Claim(JwtClaimTypes.Role, talent.type.ToString()));
            }

            //定义发行人issuer
            string iss = "sxkid.com";
            //定义受众人audience
            string aud = "api.auth";
            var nbf = DateTime.UtcNow;
            var Exp = DateTime.UtcNow.AddHours(3600);
            string sign = "q2xiARx$4x3TKqBJ";
            var secret = Encoding.UTF8.GetBytes(sign);
            var key = new SymmetricSecurityKey(secret);
            var signcreds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var jwt = new JwtSecurityToken(issuer: iss, audience: aud, claims: claims, notBefore: nbf, expires: Exp, signingCredentials: signcreds);
            var JwtHander = new JwtSecurityTokenHandler();
            var token = JwtHander.WriteToken(jwt);
            return token;
        }


    }
}
