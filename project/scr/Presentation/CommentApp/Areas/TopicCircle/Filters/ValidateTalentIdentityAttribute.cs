using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using PMS.TopicCircle.Application.Dtos;
using PMS.TopicCircle.Application.Services;
using PMS.UserManage.Application.IServices;
using Sxb.Web.Controllers;
using Sxb.Web.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Web.Areas.TopicCircle.Filters
{
    /// <summary>
    /// 校验达人身份
    /// </summary>
    public class ValidateTalentIdentityAttribute : ActionFilterAttribute
    {
        public ValidateTalentIdentityAttribute()
        {
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            ITalentService _baseService = context.HttpContext.RequestServices.GetService(typeof(ITalentService)) as ITalentService;
            ControllerBase controller = context.Controller as ControllerBase;
            Guid userId = controller.GetUserInfo().UserId;
            var isTalent = _baseService.IsTalent(userId.ToString());
            if (!isTalent)
            {
                context.Result = new JsonResult(ResponseResult.Failed("您不是达人, 不能操作"));
            }
        }
    }
}
