using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.MediatR.Events.PaidQA
{
    public  class TalentFirstReplyEvent : INotification
    {

        public Guid OrderID { get; set; }
        public TalentFirstReplyEvent(Guid orderID)
        {
            OrderID = orderID;
        }
    }
}
