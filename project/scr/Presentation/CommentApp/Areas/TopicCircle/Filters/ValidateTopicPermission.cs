using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using PMS.TopicCircle.Application.Dtos;
using PMS.TopicCircle.Application.Services;
using Sxb.Web.Areas.TopicCircle.Models;
using Sxb.Web.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Web.Areas.TopicCircle.Filters
{


    /// <summary>
    /// 帖子权限
    /// </summary>
    public enum TopicPermission
    {
        Read=1,
        Create=2,
        Udate=4,
        Delete=8
    }


    /// <summary>
    /// 校验帖子操作权限
    /// </summary>
    public class ValidateTopicPermission : ActionFilterAttribute
    {
        public string TopicIdParam { get; set; }

        /// <summary>
        /// 定义所需校验的权限
        /// </summary>
        TopicPermission Permission;

        public ValidateTopicPermission(TopicPermission permission, string topicIdParam = "Id")
        {
            Permission = permission;
            this.TopicIdParam = topicIdParam;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {

            ControllerBase controller = context.Controller as ControllerBase;
            Guid userId = controller.GetUserInfo().UserId;
            if (Permission.HasFlag(TopicPermission.Read))
            {
                ITopicService _topicService = context.HttpContext.RequestServices.GetService(typeof(ITopicService)) as ITopicService;
                if (Guid.TryParse(context.HttpContext.Request.Param(TopicIdParam), out Guid topicId))
                {


                    var topic = _topicService.Get(topicId);
                    if (topic == null)
                    {
                        context.Result = new JsonResult(ResponseResult.Failed("找不到该话题"));
                        return;
                    }
                    //校验是否有读权限
                    if (topic.OpenUserId != null)
                    {
                        //只有仅圈主可见的帖子才需要校验
                        if (topic.Creator != userId && topic.OpenUserId != userId)
                        {
                            context.Result = new JsonResult(
                                ResponseResult.Failed(ResponseCode.NoAuth,"您无话题读取权限",new { 
                                circleId = topic.CircleId})
                                );
                            return;
                        }
                    }
                }
                else
                {
                    context.Result = new JsonResult(ResponseResult.Failed("无效topicID"));
                    return;
                }


            }
            if (Permission.HasFlag(TopicPermission.Create))
            {
                //校验是否有创建权限
               TopicAddRequest topicAddDto = context.ActionArguments["topicAddDto"] as TopicAddRequest;
               ICircleService _circleService = context.HttpContext.RequestServices.GetService(typeof(ICircleService)) as ICircleService;
               AppServiceResultDto result = _circleService.CheckPermission(new CircleCheckPermissionRequestDto()
                {
                    CircleId = topicAddDto.CircleId,
                    UserId = userId
                });
                if (!result.Status)
                {
                    //非圈主则需要校验是否加入圈子
                    ICircleFollowerService _circleFollowerService = context.HttpContext.RequestServices.GetService(typeof(ICircleFollowerService)) as ICircleFollowerService;
                    bool isFollow = _circleFollowerService.CheckIsFollow(new PMS.TopicCircle.Application.Dtos.CheckIsFollowRequestDto()
                    {
                        CircleId = topicAddDto.CircleId,
                        UserId = userId
                    }); ;

                    if (!isFollow)
                    {
                        context.Result = new JsonResult(ResponseResult.Failed(ResponseCode.NoAuth,"您无新增帖子权限"));
                        return;
                    }
                }



            }
            if (Permission.HasFlag(TopicPermission.Delete))
            {
                //校验是否有删除权限

            }




        }

    }
}
