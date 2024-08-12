using Microsoft.AspNetCore.Mvc;
using Sxb.Web.Common;
using Sxb.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Web.Areas.TopicCircle
{
    public static class ControllerBaseExtension
    {

        public static UserIdentity GetUserInfo(this ControllerBase controller)
        {
           return controller.User.Identity.GetUserInfo();
        }
    }
}
