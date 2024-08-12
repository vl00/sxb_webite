using System;
using System.Collections.Generic;
using System.Text;

namespace ProductManagement.API.Http.Model.FinanceCenter
{
    /// <summary>
    /// 订单类型
    /// </summary>
    public enum OrderTypeEnum
    {
        /// <summary>
        /// 1 上学问
        /// </summary>
        Ask = 1,

        /// <summary>
        /// 2 完成提现(结算)
        /// </summary>
        Withdraw = 2,

        /// <summary>
        /// 3 机构
        /// </summary>
        Org = 3,
    }

    public class AddPayOrderRequest
    {

        /// <summary>
        /// 用户
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// 订单编号
        /// </summary>
        public Guid OrderId { get; set; }

        /// <summary>
        /// 转单旧订单ID
        /// </summary>
        public Guid? TOrderId { get; set; }

        public string OrderNo { get; set; }

        /// <summary>
        /// 订单类型
        /// </summary>
        public OrderTypeEnum OrderType { get; set; }
    
        /// <summary>
        /// 总金额
        /// </summary>
        public decimal TotalAmount { get; set; }
        /// <summary>
        /// 支付金额
        /// </summary>
        public decimal PayAmount { get; set; }

        /// <summary>
        /// 折扣金额
        /// </summary>
        public decimal DiscountAmount { get; set; }

        /// <summary>
        /// 已退款金额
        /// </summary>
        public int RefundAmount { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        public Orderbyproduct[] OrderByProducts { get; set; }
        public string OpenId { get; set; }
        public string Attach { get; set; }
        /// <summary>
        /// 来源系统
        /// </summary>
        public int System { get; set; } = 0;

        public int NoNeedPay { get; set; }
        /// <summary>
        /// 是否重新支付
        /// </summary>
        public int IsRepay { get; set; }

        /// <summary>
        /// 0 h5jsapi支付 1小程序支付 2h5支付
        /// </summary>
        public int IsWechatMiniProgram { get; set; }

    }

    public class Orderbyproduct
    {
        /// <summary>
        /// 商品金额
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        ///状态 
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// 产品类型
        /// </summary>
        public int ProductType { get; set; }
        /// <summary>
        /// 产品id
        /// </summary>
        public Guid ProductId { get; set; }
        /// <summary>
        /// 产品备注
        /// </summary>
        public string Remark { get; set; }
    }

}
