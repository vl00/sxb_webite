using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Sxb.UserCenter.Controllers
{
    public class FeedbackController : Controller
    {
        public IActionResult Index()
        {
            return RedirectToAction("Feedback", "Contact");
        }
    }
}