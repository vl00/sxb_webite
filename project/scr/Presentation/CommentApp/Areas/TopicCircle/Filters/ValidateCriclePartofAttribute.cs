using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using PMS.TopicCircle.Application.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sxb.Web.Response;

namespace Sxb.Web.Areas.TopicCircle.Filters
{
    /// <summary>
    /// 校验当前用户是否为圈子的一部分
    /// </summary>
    public class ValidateCriclePartofAttribute:ActionFilterAttribute
    {
        public string CircleIDParmName { get; set; } = "circleID";


        public override void OnActionExecuting(ActionExecutingContext context)
        {
            ICircleFollowerService _circleFollowerService = context.HttpContext.RequestServices.GetService(typeof(ICircleFollowerService)) as ICircleFollowerService;
            ControllerBase controller = context.Controller as ControllerBase;
            Guid userId = controller.GetUserInfo().UserId;
            Guid circleID = Guid.Parse(context.HttpContext.Request.Param(CircleIDParmName));
            bool isFollow =  _circleFollowerService.CheckIsFollow(new PMS.TopicCircle.Application.Dtos.CheckIsFollowRequestDto() {
                CircleId = circleID,
               UserId = userId
            }); ; 

            if (!isFollow)
            {
                context.Result = new JsonResult(ResponseResult.Failed("你未加入该圈子,无法操作."));
            }
        }
    }
}
