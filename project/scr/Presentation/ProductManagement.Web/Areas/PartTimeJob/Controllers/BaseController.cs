using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ProductManagement.Web.Areas.PartTimeJob.Models;

namespace ProductManagement.Web.Areas.PartTimeJob.Controllers
{
    public class BaseController : Controller
    {
        public PartTimeJobAdminVo _admin;
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (_admin == null)
                {
                    _admin = new PartTimeJobAdminVo()
                    {
                        Id = Guid.Parse(((System.Security.Claims.ClaimsIdentity)User.Identity).Claims.ToList()[1].Value),
                        Name = ((System.Security.Claims.ClaimsIdentity)User.Identity).Claims.ToList()[0].Value,
                        Role = ((System.Security.Claims.ClaimsIdentity)User.Identity).Claims.ToList()[2].Value,
                        Phone = ((System.Security.Claims.ClaimsIdentity)User.Identity).Claims.ToList()[3].Value,
                        InvitationCode =  ((System.Security.Claims.ClaimsIdentity)User.Identity).Claims.ToList()[4].Value
                    };
                }
                ViewBag._admin = _admin;
            }
            //else
            //{
            //    if (!context.HttpContext.Request.Path.ToString().Contains("/PartTimeJob/Home/Index"))
            //    {
            //        context.HttpContext.Response.Redirect("/PartTimeJob/Home/Index");
            //    }
            //}

        }


    }
}