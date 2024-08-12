using MediatR;
using Microsoft.Extensions.Logging;
using PMS.MediatR.Events.PaidQA;
using PMS.PaidQA.Application.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PMS.MediatR.Handle.PaidQA
{

    /// <summary>
    /// 专家首次回复事件处理
    /// </summary>
    public class TalentFirstReplyEventHandler : INotificationHandler<TalentFirstReplyEvent>
    {
        ILogger _logger;

        IServiceProvider _serviceProvider;

        IOrderService _orderService;
        IMediator _mediator;

        public TalentFirstReplyEventHandler(
            ILogger<OrderChangeEventHandler> logger
            , IOrderService orderService
            , IServiceProvider serviceProvider, IMediator mediator)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _orderService = orderService;
            _mediator = mediator;
        }


        public async Task Handle(TalentFirstReplyEvent notification, CancellationToken cancellationToken)
        {
            try {
                //await _orderService.SetToCache(notification.OrderID);
                await _mediator.Publish(new OrderChangeEvent(notification.OrderID));
            }
            catch (Exception ex) {
                _logger.LogError(ex, "专家首次回复事件处理异常。");
            }
        }
    }
}
