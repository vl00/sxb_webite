using MediatR;
using PMS.PaidQA.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.MediatR.Events.PaidQA
{

    /// <summary>
    /// 用户拒绝转单事件
    /// </summary>
    public class RefusTransitingOrderEvent : INotification
    {
        public Guid OrderID { get;  }

        public RefusTransitingOrderEvent(Guid orderId)
        {
            OrderID = orderId;
        }
    }
}
