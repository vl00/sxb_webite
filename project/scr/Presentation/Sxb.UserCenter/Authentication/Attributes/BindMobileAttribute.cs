using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using PMS.UserManage.Application.IServices;
using Sxb.UserCenter.Response;
using Sxb.UserCenter.Utils;

namespace Sxb.UserCenter.Authentication.Attributes
{
    /// <summary>
    /// 检查当前登录用户是否绑定手机号
    /// </summary>
    public class BindMobileAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.HttpContext.User.Identity.IsAuthenticated)
            {
                context.Result = new JsonResult(ResponseResult.Failed("未登录", new { ErrorCode = 300001 }));
            }
            var accountService = context.HttpContext.RequestServices.GetService(typeof(IAccountService)) as IAccountService;
            if (!accountService.IsBindPhone(context.HttpContext.User.Identity.GetId()))
            {
                context.Result = new JsonResult(ResponseResult.Failed("未绑定手机号",new { ErrorCode = 300002}));
            }
            if (context.Result == null) base.OnActionExecuting(context);
        }
    }
}
