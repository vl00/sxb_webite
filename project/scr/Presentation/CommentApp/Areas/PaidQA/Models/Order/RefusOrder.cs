using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Web.Areas.PaidQA.Models.Order
{
    public class RefusOrderRequest
    {

        [Description("订单ID")]
        public Guid OrderID { get; set; }
    }

    public class RefusOrderResult
    {
        public OrderInfoResult OrderInfo { get; set; }
    }
}
