using AutoMapper.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Sxb.Web.Middleware
{
    public class CommentAndQuestionMiddleware
    {
        private readonly RequestDelegate _next;

        public CommentAndQuestionMiddleware(RequestDelegate next) 
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext) 
        {
            if (httpContext.Request.Path.Value.Contains("commentwriter") || httpContext.Request.Path.Value.Contains("commentquestionview")) 
            {
                var request = httpContext.Request;
                var queryString = request.QueryString.Value;

                request.QueryString = new QueryString(queryString.Replace("&amp;", "&"));
            }

            await _next(httpContext);
        }
    }

    public static class CQMiddlewareExtensions 
    {
        public static IApplicationBuilder UseCommentQuestionUrlMiddleware(this IApplicationBuilder builder) 
        {
            return builder.UseMiddleware<CommentAndQuestionMiddleware>();
        }
    }
}
