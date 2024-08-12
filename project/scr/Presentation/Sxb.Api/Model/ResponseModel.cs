using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Api.Model
{

    /// <summary>
    /// 响应模型
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ResponseModel<T> where T : class
    {
        public int errCode { get; set; }
        public string msg { get; set; }
        public T data { get; set; }
    }
}
