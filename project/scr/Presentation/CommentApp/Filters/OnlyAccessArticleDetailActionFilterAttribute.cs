using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Web.Filters
{
    public class OnlyAccessArticleDetailActionFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {

          string controller =  context.ActionDescriptor.RouteValues["controller"];
            string actionName = context.ActionDescriptor.RouteValues["action"];
            if (!controller.Equals("article", StringComparison.CurrentCultureIgnoreCase) || !actionName.Equals("detail", StringComparison.CurrentCultureIgnoreCase))
            {
                context.Result = new NotFoundResult();
            }

            base.OnActionExecuting(context);
        }
    }
}
