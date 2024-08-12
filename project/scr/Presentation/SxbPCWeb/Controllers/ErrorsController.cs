using System;
using System.ComponentModel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace Sxb.PCWeb.Controllers
{
    public class ErrorsController : Controller
    {
        public ErrorsController()
        {
        }

        [Route("errors/{statusCode}")]
        [Description("错误页")]
        public IActionResult CustomError(int statusCode)
        {
            ViewBag.StatusCode = statusCode;

            if (statusCode == 404)
            {
                var callback = Request.Headers["Referer"].ToString();
                if (callback.Contains("//m.sxkid.com")|| callback.Contains("//m3.sxkid.com")|| callback.Contains("//m4.sxkid.com"))
                {
                    return Redirect("/errors/qrcode?url="+ Uri.EscapeDataString(callback));
                }
            }

            return View();
        }


        public IActionResult QrCode()
        {
            return View();
        }

        public IActionResult Test()
        {
            throw new Exception();
        }
    }
}
