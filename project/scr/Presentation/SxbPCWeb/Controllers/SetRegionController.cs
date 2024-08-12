using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Sxb.PCWeb.Common;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace Sxb.PCWeb.Controllers
{
    public class CController : Controller
    {
        /// <summary>
        /// 设置区域并跳转回去
        /// </summary>
        /// <param name="cityCode">城市代码</param>
        /// <returns></returns>
        [HttpGet]
        [Description("设置区域并跳转回去")]
        [Route("/c-{cityCode}")]
        public ActionResult Detail(int cityCode,string callback)
        {
            if (cityCode == 0)
            {
                cityCode = Request.GetLocalCity();
            }
            string redirectUrl = HttpContext.Request.Headers.TryGetValue("Referer", out StringValues url) ? url.ToString() : "/";
            HttpContext.Response.SetLocalCity(cityCode.ToString(), Request.Host.Host);
            if (!string.IsNullOrWhiteSpace(callback))
            {
                redirectUrl = callback;
            }

            Regex regex = new Regex(@"city_([0-9]{6})", RegexOptions.IgnoreCase);
            redirectUrl = regex.Replace(redirectUrl, $"city_{cityCode}");
            return Redirect(redirectUrl);
        }
    }
}
