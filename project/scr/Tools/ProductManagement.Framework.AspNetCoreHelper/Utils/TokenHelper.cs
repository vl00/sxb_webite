using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using ProductManagement.Framework.AspNetCoreHelper.ResponseModel;
using ProductManagement.Framework.Foundation;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ProductManagement.Framework.AspNetCoreHelper.Utils
{
    public class TokenHelper
    {
        public static SymmetricSecurityKey SecurityKey => new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ConstantValue.JwtKey));

        public static string GetToken(Claim[] claims, DateTime? utcExpires = null)
        {
           DateTime expires = utcExpires == null ? DateTime.UtcNow.AddMinutes(30) : utcExpires.Value;

            var creds = new SigningCredentials(SecurityKey, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                    issuer: ConstantValue.JwtIssuer,
                    audience: ConstantValue.JwtAudience,
                    claims: claims,
                    expires: expires,
                    signingCredentials: creds
                );
            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            return tokenString;
        }


        public static bool ValidateToken(string tokenString)
        {
            var handler = new JwtSecurityTokenHandler();
            //var token = handler.ReadJwtToken(tokenString);

            var paramerters = new TokenValidationParameters
            {
                ValidIssuer = ConstantValue.JwtIssuer,
                ValidAudience = ConstantValue.JwtAudience,
                IssuerSigningKey = SecurityKey
            };

            try
            {
                var principal = handler.ValidateToken(tokenString, paramerters, out _);
                return principal.Identity.IsAuthenticated;
            }
            catch (SecurityTokenExpiredException)
            {
                //过期
            }
            catch (Exception)
            {
                //其他
            }
            return false;
        } 


        public static ClaimsPrincipal ValidateTokenTest(string tokenString)
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(tokenString);

            SecurityToken securityToken = null;
            var paramerters = new TokenValidationParameters();
            paramerters.IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ConstantValue.JwtKey));
            paramerters.ValidIssuer = ConstantValue.JwtIssuer;
            paramerters.ValidAudience = ConstantValue.JwtAudience;

            try
            {
                var principal = handler.ValidateToken(tokenString, paramerters, out securityToken);
                return principal;
            }
            catch (SecurityTokenExpiredException)
            {
                //过期
            }
            catch (Exception)
            {
                //其他
            }
            return default;
        }
    }
}
