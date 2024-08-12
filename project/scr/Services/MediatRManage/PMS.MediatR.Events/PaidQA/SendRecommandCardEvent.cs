using MediatR;
using PMS.PaidQA.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.MediatR.Events.PaidQA
{
    public class SendRecommandCardEvent: INotification
    {
        public Order Order { get; set; }
        public SendRecommandCardEvent(Order order)
        {
            Order = order;
        }
    }
}
