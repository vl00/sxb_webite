using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PMS.MediatR.Events.PaidQA;
using PMS.PaidQA.Application.Services;
using PMS.PaidQA.Domain.Entities;
using PMS.PaidQA.Domain.Enums;
using PMS.PaidQA.Domain.Message;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PMS.MediatR.Handle.PaidQA
{
    public class RefusTransitingOrderEventHandler : INotificationHandler<RefusTransitingOrderEvent>
    {
        ILogger _logger;

        IServiceProvider _serviceProvider;

        public RefusTransitingOrderEventHandler(IServiceProvider serviceProvider, ILogger<OverTimeOrderHandler> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task Handle(RefusTransitingOrderEvent notification, CancellationToken cancellationToken)
        {
            try {
                using (var scope = _serviceProvider.CreateScope())
                {
                    IMessageService messageService = _serviceProvider.GetRequiredService<IMessageService>();
                    IOrderService orderService = _serviceProvider.GetRequiredService<IOrderService>();
                    Order order = await orderService.GetAsync(notification.OrderID);
                    //提问完后给用户友好提醒消息
                    SystemMessage friendlyMsg = new SystemMessage()
                    {
                        Content = "请您再耐心等候一会儿，专家看到消息后将会尽快回复您！如果专家超过24小时还未进行回复，我们将会全额退款。"
                    };
                    Message friendlyMsgToUser = friendlyMsg.CreateMessage(default(Guid), order.CreatorID, order.ID, MsgType.System);
                    await messageService.SendMessage(friendlyMsgToUser, friendlyMsg);
                }

            } catch (Exception ex)
            {
                _logger.LogError(ex, "拒绝转单事件处理失败。");
            }
        }
    }
}
