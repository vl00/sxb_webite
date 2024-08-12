using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using PMS.TopicCircle.Application.Dtos;
using PMS.TopicCircle.Application.Services;
using Sxb.Web.Controllers;
using Sxb.Web.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Web.Areas.TopicCircle.Filters
{
    /// <summary>
    /// 校验圈主身份,用于限制某些仅限圈主操作自己圈子的Action
    /// </summary>
    public class ValidateCircleMasterAttribute : ActionFilterAttribute
    {
        public string CircleIDParmName { get; set; } = "circleID";


        public ValidateCircleMasterAttribute()
        {
        }


        public override void OnActionExecuting(ActionExecutingContext context)
        {
            ICircleService _circleService = context.HttpContext.RequestServices.GetService(typeof(ICircleService)) as ICircleService;
            ControllerBase controller = context.Controller as ControllerBase;
            Guid userId = controller.GetUserInfo().UserId;
            if (Guid.TryParse(context.HttpContext.Request.Param(CircleIDParmName), out Guid circleID))
            {
                AppServiceResultDto result = _circleService.CheckPermission(new CircleCheckPermissionRequestDto()
                {
                    CircleId = circleID,
                    UserId = userId
                });
                if (!result.Status)
                {
                    context.Result = new JsonResult(ResponseResult.Failed(result.Msg));
                }
            }
            else {
                context.Result = new JsonResult(ResponseResult.Failed("无效circleID"));
            }



        }
    }
}
