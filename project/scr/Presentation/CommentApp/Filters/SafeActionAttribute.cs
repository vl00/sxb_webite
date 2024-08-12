using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ProductManagement.Framework.Cache.Redis;
using Sxb.Web.RequestModel;
using Sxb.Web.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Web.Filters
{
    /// <summary>
    /// 一个安全的action操作应该是有序的而不是一拥而上。
    /// </summary>
    public class SafeActionAttribute : ActionFilterAttribute
    {
        public string SafeActionRequestParamName { get; set; } = "request";

        public int WillExcuteSecond { get; set; } = 5;
        string lockKey = "";
        string lockVal = "";
        IEasyRedisClient _easyRedisClient;
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            _easyRedisClient = context.HttpContext.RequestServices.GetService(typeof(IEasyRedisClient)) as IEasyRedisClient;
            SafeActionRequest request = context.ActionArguments[SafeActionRequestParamName] as SafeActionRequest;
            if (request != null)
            {
                lockKey = request.LockKey(context.HttpContext);
                lockVal = request.LockValue(context.HttpContext);
                TimeSpan maxHandleTimeSpan = TimeSpan.FromSeconds(WillExcuteSecond);
                if (!(_easyRedisClient.LockTakeAsync(lockKey, lockVal, maxHandleTimeSpan).GetAwaiter().GetResult()))
                {
                    context.Result = new JsonResult(ResponseResult.Failed("操作过于频繁。"));

                }

            }

            base.OnActionExecuting(context);
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            _easyRedisClient.LockReleaseAsync(lockKey, lockVal).GetAwaiter().GetResult(); 
            base.OnActionExecuted(context);
        }
    }
}
