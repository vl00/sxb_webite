using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using PMS.TopicCircle.Application.Services;
using Sxb.Web.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Web.Filters
{
    /// <summary>
    /// 校验上学帮内部通讯，上学帮互相调用时可携带上sxbinnertoken，该筛选器对token进行校验
    /// </summary>
    public class ValidateSxbInnerTokenAttribute : ActionFilterAttribute
    {
        string key = "sxbinnertoken";
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var kvService = context.HttpContext.RequestServices.GetService(typeof(IKeyValueService)) as IKeyValueService;
            string token = kvService.Get(key);
            string htoken = context.HttpContext.Request.Headers[key].ToString();
            if (!string.Equals(token, htoken, StringComparison.CurrentCultureIgnoreCase))
            {
                //无效token值
                ResponseResult result = new ResponseResult()
                {
                    status = ResponseCode.UnAuth,
                    Msg = "仅限内部调用，当前为无效Token",
                    Succeed = false
                };
                context.Result = new JsonResult(result);
            }
        }
    }
}
