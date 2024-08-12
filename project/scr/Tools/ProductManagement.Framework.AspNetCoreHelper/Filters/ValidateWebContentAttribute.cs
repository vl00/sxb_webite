using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using ProductManagement.Framework.Foundation;
using ProductManagement.Framework.AspNetCoreHelper.RequestModel;
using ProductManagement.Framework.AspNetCoreHelper.ResponseModel;
using ProductManagement.API.Aliyun;
using ProductManagement.API.Aliyun.Model;

namespace ProductManagement.Framework.AspNetCoreHelper.Filters
{
    public class ValidateWebContentAttribute: ActionFilterAttribute
    {
        public string ContentParam { get; set; } = "content";

        public override void OnActionExecuting(ActionExecutingContext context)
        {

            var param = context.ActionArguments[ContentParam] as WebContentRequest;
            IText text = context.HttpContext.RequestServices.GetService(typeof(IText)) as IText;

            string content = param.GetContent();
            if (!string.IsNullOrEmpty(content))
            {
                //检测敏感词
                if (!text.Check(new ProductManagement.API.Aliyun.Model.GarbageCheckRequest()
                {
                    scenes = new[] { "antispam" },
                    tasks = new List<ProductManagement.API.Aliyun.Model.GarbageCheckRequest.Task>() {
                      new GarbageCheckRequest.Task(){
                         content= content,
                         dataId = Guid.NewGuid().ToString()
                      }
                      }
                }).Result
                )
                {
                    //包含敏感词
                    context.Result = new JsonResult(new ResponseResult()
                    {
                        Msg = ResponseCode.GarbageContent.Description(),
                        status = ResponseCode.GarbageContent,
                        Succeed = false
                    });

                }
            }

        }



    }
}
