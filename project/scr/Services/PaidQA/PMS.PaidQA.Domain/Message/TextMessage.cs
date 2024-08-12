using PMS.PaidQA.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.PaidQA.Domain.Message
{

    /// <summary>
    /// 文字消息
    /// </summary>
    public class TextMessage:PaidQAMessage
    {
        public string Content { get; set; }

        protected override MsgMediaType MediaType => MsgMediaType.Text;
    }
}
