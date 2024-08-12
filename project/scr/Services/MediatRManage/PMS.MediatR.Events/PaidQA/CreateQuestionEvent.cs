using MediatR;
using PMS.PaidQA.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.MediatR.Events.PaidQA
{
   public class CreateQuestionEvent: INotification
    {
        public Order Order { get;  }
        public CreateQuestionEvent(Order order)
        {
            Order = order;
        }
    }
}
