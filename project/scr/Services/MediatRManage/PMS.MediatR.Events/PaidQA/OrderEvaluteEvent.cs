using MediatR;
using PMS.PaidQA.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.MediatR.Events.PaidQA
{
    public class OrderEvaluteEvent : INotification
    {

        public Order Order { get; set; }
        public OrderEvaluteEvent(Order order)
        {
            this.Order = order;
        }
    }
}
