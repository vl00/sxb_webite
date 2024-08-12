using MediatR;
using PMS.MediatR.Request.PaidQA;
using PMS.PaidQA.Application.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PMS.MediatR.Handle.PaidQA
{
    public class UpdateOrderCacheHandler : IRequestHandler<UpdateOrderCacheRequest, bool>
    {
        IOrderService _orderService;

        public UpdateOrderCacheHandler(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public async Task<bool> Handle(UpdateOrderCacheRequest request, CancellationToken cancellationToken)
        {
            return false;
        }
    }

}
