using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace PMS.PaidQA.Domain.Enums
{
    public enum MsgType
    {
        [Description("系统消息")]
        System = 1,
        [Description("客服消息")]
        Custom = 2,
        [Description("提问")]
        Question = 3,
        [Description("回答")]
        Answer = 4,
        [Description("追问")]
        GoAsk = 5,
    }
}
