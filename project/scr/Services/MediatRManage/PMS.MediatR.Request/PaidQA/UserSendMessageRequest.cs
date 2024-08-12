using MediatR;
using PMS.PaidQA.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace PMS.MediatR.Request.PaidQA
{
    public class UserSendMessageRequest : IRequest<bool>
    {
        public Order Order { get; }
        public UserSendMessageRequest(Order order)
        {
            Order = order;
        }
    }
}
