using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using Sxb.Web.Response;
using System;
using System.Threading.Tasks;

namespace Sxb.Web.Controllers
{
    [OpenApiIgnore]
    public class NetworkCheckController : Controller
    {

        public IActionResult Index()
        {
            //ViewBag.CurrentIP = Request.Server;
            return View();
        }

        [HttpPost]
        public ResponseResult CheckApi()
        {
            return ResponseResult.Success(DateTime.Now);
        }
    }
}