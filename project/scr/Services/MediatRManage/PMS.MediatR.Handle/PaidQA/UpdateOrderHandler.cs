using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PMS.MediatR.Events;
using PMS.MediatR.Events.PaidQA;
using PMS.MediatR.Request.PaidQA;
using PMS.PaidQA.Application.Services;

namespace PMS.MediatR.Handle.PaidQA
{
    public class UpdateOrderHandler : INotificationHandler<UpdateOrderEvent>
    {
        IOrderService _orderService;

        public UpdateOrderHandler(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public async Task Handle(UpdateOrderEvent notification, CancellationToken cancellationToken)
        {
             await Task.CompletedTask;
        }
    }
   
}
