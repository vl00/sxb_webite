using System;


namespace Sxb.PCWeb
{
	public static class IPUtil
	{
		public static String GetIpAddr(Microsoft.AspNetCore.Http.HttpRequest request)
		{
			String ip = request.Headers["X-Forwarded-For"].ToString(); if (ip == null || ip.Length == 0 || "unknown".Equals(ip)) { ip = request.Headers["Proxy-Client-IP"].ToString(); }
			if (string.IsNullOrEmpty(ip)|| "unknown".Equals(ip)) { ip = request.Headers["WL-Proxy-Client-IP"].ToString(); }
			if (string.IsNullOrEmpty(ip)|| "unknown".Equals(ip)) { ip = request.Headers["HTTP_CLIENT_IP"].ToString(); }
			if (string.IsNullOrEmpty(ip)|| "unknown".Equals(ip)) { ip = request.Headers["HTTP_X_FORWARDED_FOR"].ToString(); }
			if (string.IsNullOrEmpty(ip)|| "unknown".Equals(ip)) { ip = request.Headers["x-real-ip"].ToString(); }
			if (string.IsNullOrEmpty(ip)|| "unknown".Equals(ip)) { ip = request.Host.Host; }
			String[] ips = ip.Split(","); if (ips.Length > 1) { ip = ips[0]; }
			return ip;
		}
	}
}
