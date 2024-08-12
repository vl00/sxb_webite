using MediatR;
using PMS.PaidQA.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.MediatR.Events.PaidQA
{
    public class OverTimeOrderEvent : INotification
    {

        public Order Order { get; }

        public OverTimeOrderEvent(Order order)
        {
            Order = order;
        }
    }
}
