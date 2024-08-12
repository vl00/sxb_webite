using PMS.PaidQA.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using PMS.PaidQA.Domain.Entities;
using Sxb.Web.Areas.PaidQA.Models.Evaluate;

namespace Sxb.Web.Areas.PaidQA.Models.Order
{
    public class GetOrderRequest
    {
        [Description("订单ID")]
        public Guid OrderID { get; set; }

        [Description("订单状态")]
        public OrderStatus OrginStatus { get; set; }

        [Description("轮询次数")]
        public int RepeatTime { get; set; } = 5;
    }
    public class GetOrderResult
    {
        public OrderInfoResult OrderInfo { get; set; }

        [Description("评价信息")]
        public EvaluateResult EvaluateInfo { get; set; }

    }
}
