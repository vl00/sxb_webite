using Newtonsoft.Json;
using PMS.PaidQA.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.PaidQA.Domain.Message
{
    public class SystemStatuMessage:PaidQAMessage
    {
        public string Content { get; set; } = "本次咨询已结束";

        protected override MsgMediaType MediaType => MsgMediaType.SystemStatu;


    }
}
