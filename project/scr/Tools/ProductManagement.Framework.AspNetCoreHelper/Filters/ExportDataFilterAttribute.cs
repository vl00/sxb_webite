using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ProductManagement.Framework.AspNetCoreHelper.RequestModel;
using ProductManagement.Framework.AspNetCoreHelper.ResponseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductManagement.Framework.AspNetCoreHelper.Filters
{
    public class ExportDataFilterAttribute: ActionFilterAttribute
    {
        public string ExportDataToMailRequestParamName { get; set; }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            ExportDataToMailRequest request = context.ActionArguments[ExportDataToMailRequestParamName] as ExportDataToMailRequest;

            foreach (var mail in request.MainMails)
            {
                if (!mail.Contains("@sxkid.com", StringComparison.CurrentCultureIgnoreCase))
                {
                    context.Result = new JsonResult(ResponseResult.Failed("包含非法邮件地址"));
                }
            }
            foreach (var mail in request.CCMails)
            {
                if (!mail.Contains("@sxkid.com", StringComparison.CurrentCultureIgnoreCase))
                {
                    context.Result = new JsonResult(ResponseResult.Failed("包含非法邮件地址"));
                }
            }

    

            base.OnActionExecuting(context);
        }
    }
}
