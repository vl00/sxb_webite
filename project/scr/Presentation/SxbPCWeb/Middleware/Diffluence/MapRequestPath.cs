using System;
using Microsoft.AspNetCore.Http;

namespace Sxb.PCWeb.Middleware.Diffluence
{
	/// <summary>
	/// 实现读取配置文件的映射路径
	/// </summary>
	public class MapRequestPath : IRequestPath
	{
		public string Method => "GET";

		public PathString OrientPath => string.Empty;

        public QueryString Query => QueryString.Empty;
    }
}
