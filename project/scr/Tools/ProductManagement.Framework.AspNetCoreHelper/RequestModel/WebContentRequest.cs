using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductManagement.Framework.AspNetCoreHelper.RequestModel
{
    /// <summary>
    /// 带有Web Content的请求可继承该类，结合ValidateWebContentAttribute做垃圾网络文本检测
    /// </summary>
    public abstract class WebContentRequest
    {
        public abstract string GetContent();
    }
}
