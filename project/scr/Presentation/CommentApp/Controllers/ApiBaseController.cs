using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Sxb.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public abstract class ApiBaseController : ControllerBase
    {
        protected Guid? UserId { get { return this.User.Identity.GetUserInfo().UserId; } }

        /// <summary>
        /// guid or empty
        /// </summary>
        protected Guid UserIdOrDefault { get { return UserId == null ? Guid.Empty : UserId.Value; } }

        protected double Latitude { get { return NumberParse.DoubleParse(Request.Cookies["latitude"], 0); } }

        protected double Longitude { get { return NumberParse.DoubleParse(Request.Cookies["longitude"], 0); } }



    }
}
