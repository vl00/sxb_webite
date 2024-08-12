using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Web.Utils
{
    public static class ShareUrlParameterJoin
    {
        /// <summary>
        ///  参数拼接【返回值带有？号】
        /// </summary>
        /// <param name="query">当前url请求参数集合</param>
        /// <param name="excludePara">排除参数</param>
        /// <returns></returns>
        public static string ParameterJoin(IQueryCollection query,List<string> excludePara) 
        {
            if (!query.Any()) 
            {
                return "";
            }

            string url = "?";
            foreach (var key in query.Keys)
            {
                //排除不需要的参数 其余全部进行拼接
                if (!excludePara.Contains(key)) 
                {
                    url += $"{key}={query[key]}&";
                }
            }
            return url.Remove(url.Length - 1, 1);
        }

    }
}
