using MediatR;
using PMS.PaidQA.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.MediatR.Events.PaidQA
{

    /// <summary>
    /// 拒绝订单事件。
    /// </summary>
    public class RefusOrderEvent : INotification
    {
        public Order Order { get; }
        public RefusOrderEvent(Order order)
        {
            Order = order;

        }
    }
}
