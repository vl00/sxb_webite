using System;
using System.Collections.Generic;
using System.Text;

namespace ProductManagement.API.Http.Request.WXWork
{
    public class GetAddCustomerQrCodeRequest
    {
        /// <summary>
        /// 顾问用户Union ID
        /// </summary>
        public string AdviserId { get; set; }


        /// <summary>
        /// 顾问用户ID
        /// </summary>

        public string AdviserUserId { get; set; }


        /// <summary>
        /// 发展人用户ID
        /// </summary>
        public string InviterId { get; set; }

        /// <summary>
        /// 发展人的OpenId
        /// </summary>
        public string InviterOpenId { get; set; }
    }
}
