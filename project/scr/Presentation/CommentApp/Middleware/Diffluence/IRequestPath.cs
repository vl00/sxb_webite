using System;
using Microsoft.AspNetCore.Http;

namespace Sxb.Web.Middleware.Diffluence
{
	public interface IRequestPath
	{
		/// <summary>
		/// 请求的http方法
		/// </summary>
		string Method { get; }

		/// <summary>
		/// 重新定向的路径
		/// </summary>
		PathString OrientPath { get; }

        /// <summary>
		/// 重新请求参数
		/// </summary>
        QueryString Query { get; }
    }
}
