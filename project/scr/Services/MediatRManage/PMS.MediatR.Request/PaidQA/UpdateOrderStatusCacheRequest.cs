using MediatR;
using PMS.PaidQA.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.MediatR.Request.PaidQA
{
    public class UpdateOrderStatusCacheRequest : IRequest<bool>
    {
        public Order Order { get; set; }
        public UpdateOrderStatusCacheRequest(Order order)
        {
            Order = order;
        }
    }
}
