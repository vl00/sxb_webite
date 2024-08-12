using MediatR;
using PMS.PaidQA.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.MediatR.Request.PaidQA
{
    public  class AskWechatTemplateSendRequest : IRequest<bool>
    {
        /// <summary>
        /// 订单ID
        /// </summary>
        public Guid OrderID { get; set; }

        public string first { get; set; }
        public string keyword1 { get; set; }
        public string keyword2 { get; set; }
        public string keyword3 { get; set; }
        public string keyword4 { get; set; }
        public string remark { get; set; }
        public WechatMessageType msgtype { get; set; }
        public string  openid { get; set; }
        public string href { get; set; }
        public string user_nickname { get; set; }
        public string daren_nickname { get; set; }
    }
}
