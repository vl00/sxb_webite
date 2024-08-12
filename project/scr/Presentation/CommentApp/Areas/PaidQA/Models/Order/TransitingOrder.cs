using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Web.Areas.PaidQA.Models.Order
{
    public class TransitingOrderRequest
    {
        [Description("订单ID")]
        public Guid OriginOrderID { get; set; }

        [Description("目标专家用户ID")]
        public Guid TargetAnwserID { get; set; }
    }

    public class TransitingOrderResult
    {
        [Description("订单信息")]
        public OrderInfoResult OrderInfo { get; set; }

        [Description("新订单信息")]
        public OrderInfoResult NewOrderInfo { get; set; }
    }

}
