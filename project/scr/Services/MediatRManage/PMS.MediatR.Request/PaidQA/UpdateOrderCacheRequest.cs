using MediatR;
using PMS.PaidQA.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.MediatR.Request.PaidQA
{


    public class UpdateOrderCacheRequest : IRequest<bool>
    {
        public Order Order { get; }
        public UpdateOrderCacheRequest(Order order)
        {
            Order = order;
        }
    }
}
