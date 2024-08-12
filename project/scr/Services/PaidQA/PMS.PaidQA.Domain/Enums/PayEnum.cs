using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace PMS.PaidQA.Domain.Enums
{
    /// <summary>
    /// 支付类型
    /// </summary>
    public enum PayTypeEnum
    {

        [Description("充值")]
        Recharge = 1
    }
    /// <summary>
    /// 支付方式
    /// </summary>
    public enum PayWayEnum
    {
        [Description("微信支")]
        WeChatPay = 1,
        [Description("支付宝")]
        AliPay = 2,

    }
    /// <summary>
    /// 支付状态
    /// </summary>
    public enum PayStatusEnum
    {

        [Description("待支付")]
        InProcess = 0,
        [Description("成功")]
        Success = 1,
        [Description("失败")]
        Fail = 2
    }
}
