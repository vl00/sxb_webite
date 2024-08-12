using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Web.Areas.PaidQA.Models.Order
{
    public class OverOrderRequest
    {
        [Description("订单ID")]
        public Guid OrderID { get; set; }
    }

    public class OverOrderResult
    {
        public OrderInfoResult OrderInfo { get; set; }
    }
}
