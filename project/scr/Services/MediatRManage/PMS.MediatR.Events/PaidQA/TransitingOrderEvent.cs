using MediatR;
using PMS.PaidQA.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.MediatR.Events.PaidQA
{

    /// <summary>
    /// 转单事件
    /// </summary>
    public class TransitingOrderEvent : INotification
    {

        public decimal RefundAmount { get; set; }
        public Order OrginOrder { get;  }
        public Order NewOrder { get; }

        public TransitingOrderEvent(Order orginOrder,Order newOrder,decimal refundAmount)
        {
            OrginOrder = orginOrder;
            NewOrder = newOrder;
            RefundAmount = refundAmount;
        }
    }
}
