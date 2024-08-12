using System.ComponentModel;

namespace PMS.PaidQA.Domain.Enums
{
    public enum OrderStatus
    {
        Unknow = -99,
        [Description("支付失败")]
        PayFaile =-1,
        /// <summary>
        /// 未知
        /// </summary>
        [Description("未支付")]
        UnPay = 0,
        /// <summary>
        /// 待提问
        /// </summary>
        [Description("待提问")]
        WaitingAsk = 1,
        /// <summary>
        /// 待回答
        /// </summary>
        [Description("待回答")]
        WaitingReply = 2,
        /// <summary>
        /// 进行中
        /// </summary>
        [Description("进行中")]
        Processing = 3,
        /// <summary>
        /// 已结束
        /// </summary>
        [Description("已结束")]
        Finish = 4,
        /// <summary>
        /// 已转单
        /// </summary>
        [Description("已转单")]
        Transfered = 5,
        /// <summary>
        /// 已超时
        /// </summary>
        [Description("已超时")]
        TimeOut = 6,
        /// <summary>
        /// 已拒绝
        /// </summary>
        [Description("已拒绝")]
        Refused = 7
    }
}
