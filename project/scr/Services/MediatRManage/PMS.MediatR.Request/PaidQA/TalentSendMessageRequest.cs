using MediatR;
using PMS.PaidQA.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace PMS.MediatR.Request.PaidQA
{
    public class TalentSendMessageRequest : IRequest<bool>
    {
        public Order Order { get; }
        public TalentSendMessageRequest(Order order)
        {
            Order = order;
        }
    }
}
