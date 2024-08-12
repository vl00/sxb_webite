using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;

namespace Sxb.Web.Middleware.Diffluence
{
	public class DefaultRequestPath : IRequestPath
	{
        public const string DefealtController = "";
        public const string DefealtAction = "";

        private readonly IRequestPath _next;

        private readonly string _Controller;
        private readonly string _Action;
        private readonly QueryString _Query;

        private readonly PathString path;


        public DefaultRequestPath(HttpContext httpContext) : this(httpContext, null)
		{

		}

		public DefaultRequestPath(HttpContext httpContext, IRequestPath next)
		{
            path = httpContext.Request.Path;


            Regex regEx = new Regex(@".*~");
            var newPath = regEx.Match(path).Value;

            var resolvePath = newPath.ToLower().Split("/");


            if (resolvePath.Length > 2)
            {
                _Controller = resolvePath[1];
                _Action = ResolveAction(resolvePath[2]);
                _Query = ResolveQuery(resolvePath[2], httpContext.Request.QueryString);
            }
            else
            {
                _Controller =  DefealtController;
                _Action = ResolveAction(resolvePath[1]);
                _Query = ResolveQuery(resolvePath[1], httpContext.Request.QueryString);
            }
            _next = next;
		}



        public string ResolveAction(string path)
        {
            var resolvePath = path.ToLower().Replace("~", "").Split("_");
            if (resolvePath[0].Contains("-"))
            {
                return DefealtAction;
            }
            return resolvePath[0];
        }


        public QueryString ResolveQuery(string path,QueryString queryString)
        {
            var resolvePath = path.ToLower().Replace("~","").Split("_");

            var parameters = new List<KeyValuePair<string, string>>();

            foreach (var item in resolvePath)
            {
                var para = item.Split("-");
                if (para.Length == 2)
                {
                    string value = IsGUID(para[1]) ? Guid.Parse(para[1]).ToString() : System.Web.HttpUtility.UrlDecode(para[1]);

                    var p = new KeyValuePair<string, string>(para[0], value);
                    parameters.Add(p);
                }
            }

            if (!string.IsNullOrWhiteSpace(queryString.Value) &&
                queryString.Value.StartsWith("?", StringComparison.CurrentCultureIgnoreCase))
            {
                var resolveTranPath = queryString.Value.ToLower().Replace("?", "").Split("&");
                foreach (var item in resolveTranPath)
                {
                    var para = item.Split("=");
                    if (para.Length == 2)
                    {
                        string value = IsGUID(para[1]) ? Guid.Parse(para[1]).ToString() : System.Web.HttpUtility.UrlDecode(para[1]);

                        var p = new KeyValuePair<string, string>(para[0], value);
                        parameters.Add(p);
                    }
                }
            }


            return QueryString.Create(parameters);
        }

        private bool IsGUID(string expression)
        {
            if (expression != null)
            {
                Regex guidRegEx = new Regex(@"^([0-9a-fA-F]){32}$");
                return guidRegEx.IsMatch(expression);
            }
            return false;
        }

        /// <summary>
        /// 请求的http方法
        /// </summary>
        public string Method
		{
			get
			{
				return (_next == null) ? "get" : _next.Method;
			}
		}
		/// <summary>
		/// 重新定向的路径
		/// </summary>
		public PathString OrientPath
		{
			get
			{
				if (IsDefault())
				{
                    if (string.IsNullOrWhiteSpace(_Controller) && string.IsNullOrWhiteSpace(_Action))
                    {
                        return new PathString($"/");
                    }
                    if (string.IsNullOrWhiteSpace(_Action))
                    {
                        return new PathString($"/{_Controller}");
                    }
                    return new PathString($"/{_Controller}/{_Action}");
                }
				return (_next == null) ? PathString.Empty : _next.OrientPath;
			}
		}

        /// <summary>
		/// 重新请求参数
		/// </summary>
        public QueryString Query
        {
            get
            {
                if (IsDefault())
                {
                    return _Query;
                }
                return (_next == null) ? QueryString.Empty : _next.Query;
            }
        }

        /// <summary>
        /// 是否默认的请求方法体结构
        /// </summary>
        /// <returns></returns>
        private bool IsDefault()
		{
            return path.HasValue && path.Value.Contains("~", StringComparison.CurrentCultureIgnoreCase);

        }

	}
}
