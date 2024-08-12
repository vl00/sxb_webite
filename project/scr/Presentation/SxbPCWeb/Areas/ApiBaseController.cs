using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PMS.OperationPlateform.Domain.Entitys;

namespace Sxb.PCWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public abstract class ApiBaseController : ControllerBase
    {
        protected Guid? userId { get { return this.User.Identity.GetUserInfo().UserId; } }

        /// <summary>
        /// guid or empty
        /// </summary>
        protected Guid UserIdOrDefault { get { return userId == null ? Guid.Empty : userId.Value; } }
    }
}
