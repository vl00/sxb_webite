using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace Sxb.Web.Controllers
{
    [OpenApiIgnore]
    public class ScreenController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}