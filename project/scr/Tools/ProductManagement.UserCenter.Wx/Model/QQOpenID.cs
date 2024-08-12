using System;
using System.Collections.Generic;
using System.Text;

namespace ProductManagement.UserCenter.TencentCommon.Model
{
    public class QQOpenID : QQRet
    {
        public string client_id { get; set; }
        /// <summary>
        /// 授权用户唯一标识
        /// </summary>
        public string openid { get; set; }
        /// <summary>
        /// 授权用户唯一联合标识
        /// </summary>
        public string unionid { get; set; }
    }
}
