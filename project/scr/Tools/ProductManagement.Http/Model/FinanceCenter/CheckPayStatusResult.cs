using System;
using System.Collections.Generic;
using System.Text;

namespace ProductManagement.API.Http.Model.FinanceCenter
{
    public enum OrderStatus
    {

        /// <summary>
        /// 待支付
        /// </summary>
        Wait = 1,
        /// <summary>
        /// 进行中
        /// </summary>
        Process = 2,
        /// <summary>
        /// 已完成
        /// </summary>
        Finish = 3,
        /// <summary>
        /// 已取消
        /// </summary>
        Cancel = 4,
        /// <summary>
        /// 已退款
        /// </summary>
        Refund = 5,
        /// <summary>
        /// 支付成功
        /// </summary>
        PaySucess = 6,
        /// <summary>
        /// 支付失败
        /// </summary>
        PayFaile = 7,

    }

    public class CheckPayStatusResult
    {
        public OrderStatus OrderStatus { get; set; }
        public string wechatTradeState { get; set; }

    }
}
