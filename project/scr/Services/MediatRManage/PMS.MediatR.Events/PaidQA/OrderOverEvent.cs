using MediatR;
using PMS.PaidQA.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.MediatR.Events.PaidQA
{

    /// <summary>
    /// 订单结束事件
    /// </summary>
    public class OrderOverEvent: INotification
    {
        public Order Order { get; }

        public OrderOverEvent(Order order)
        {
            Order = order;
        }
    }
}
