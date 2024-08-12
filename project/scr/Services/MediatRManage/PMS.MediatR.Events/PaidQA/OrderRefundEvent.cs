using MediatR;
using PMS.PaidQA.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.MediatR.Events.PaidQA
{

    /// <summary>
    /// 订单退款成功事件
    /// </summary>
    public class OrderRefundEvent : INotification
    {
        public Order Order { get; set; }
        public OrderRefundEvent(Order order)
        {
            Order = order;
        }

    }
}
