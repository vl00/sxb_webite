using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.MediatR.Handle.PaidQA
{
    using global::MediatR;
    using PMS.MediatR.Request.PaidQA;
    using PMS.PaidQA.Application.Services;
    using System.Threading;
    using System.Threading.Tasks;

    public class OrderExistsRequestHandler : IRequestHandler<OrderExistsRequest, bool>
    {
        IOrderService _orderService;
        public OrderExistsRequestHandler(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public async Task<bool> Handle(OrderExistsRequest request, CancellationToken cancellationToken)
        { 
           return await  _orderService.ExistsOrderAsync(request.UserId);
        }
    }
}
