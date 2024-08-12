using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Web.RequestModel
{

    /// <summary>
    /// 安全的Action请求基类
    /// </summary>
    public abstract class SafeActionRequest
    {
        public abstract string LockKey(HttpContext context);

        public abstract string LockValue(HttpContext context);
    }
}
