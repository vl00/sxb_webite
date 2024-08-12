using System;
using System.Collections.Generic;
using System.Text;

namespace ProductManagement.UserCenter.TencentCommon.Model
{
    public class QQRet
    {
        public int ret { get; set; }
        /// <summary>
        /// 错误编号
        /// </summary>
        public int code { get; set; }
        /// <summary>
        /// 错误提示消息
        /// </summary>
        public string msg { get; set; }
    }
}
