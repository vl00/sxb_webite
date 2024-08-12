using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Api.Response
{
    /// <summary>
    /// APP响应结果模板
    /// </summary>
    public class AppResponseResult
    {
        /// <summary>
        /// 状态码
        /// </summary>
        public ResponseCode Status { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrorMsg { get; set; }
    }
}