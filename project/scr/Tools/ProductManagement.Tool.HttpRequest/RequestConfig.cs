using System;
using System.Collections.Generic;
using System.Text;

namespace ProductManagement.Tool.HttpRequest
{
    public class RequestConfig
    {
        /// <summary>
        /// 请求的服务器地址
        /// </summary>
        public string ServerUrl { get; set; }

        /// <summary>
        /// 设置一个超时时间,  秒为单位
        /// 不设置, 或者非正数, 则默认10秒超时
        /// </summary>
        public virtual int TimeOut { get; set; } = 10;

    }
}
