using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.MediatR.Request.PaidQA
{
    public class OrderExistsRequest : IRequest<bool>
    {

        public Guid UserId { get; set; }

        public OrderExistsRequest(Guid userId)
        {
            this.UserId = userId;
        }
    }
}
