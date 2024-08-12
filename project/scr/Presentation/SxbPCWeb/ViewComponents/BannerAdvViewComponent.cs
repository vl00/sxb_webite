using System;
using Microsoft.AspNetCore.Mvc;

namespace Sxb.PCWeb.ViewComponents
{
    public class BannerAdvViewComponent : ViewComponent
    {


        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
