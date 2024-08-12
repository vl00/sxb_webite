using System;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using Sxb.Web.Filters.Attributes;
using Sxb.Web.Response;

namespace Sxb.Web.Filters
{
    public class ValidateModelAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var desc = context.ActionDescriptor as ControllerActionDescriptor;
            if (desc.MethodInfo.IsDefined(typeof(UnValidateModelAttribute)) ||
                desc.ControllerTypeInfo.IsDefined(typeof(UnValidateModelAttribute))
                )
            {
                return;
            }


            if (!context.ModelState.IsValid)
            {
                var result = context.ModelState.Keys
                        .SelectMany(key =>
                        context.ModelState[key].Errors.Select(x => new ValidationError(key, x.ErrorMessage)))
                        .FirstOrDefault();
                context.Result = new JsonResult(ResponseResult.Failed(ResponseCode.ValidationError, result.Message));
            }
        }
        public class ValidationError
        {
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public string Field { get; }
            public string Message { get; }
            public ValidationError(string field, string message)
            {
                Field = field != string.Empty ? field : null;
                Message = message;
            }
        }
    }
}
