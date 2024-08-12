using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace PMS.PaidQA.Domain.Enums
{
    public enum WechatMessageType
    {
        [Description("专家回复问题")]
        专家回复问题 = 1,
        收到提问 = 2,
        问答超过时未回复=3,
        订单超时转单提醒=4,
        用户发起追问=5,
        用户发起最后一次追问=6,
        用户已评价=7

    }
}
