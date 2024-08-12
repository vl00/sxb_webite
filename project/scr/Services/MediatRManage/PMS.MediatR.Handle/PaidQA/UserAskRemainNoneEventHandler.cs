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
    public class UserAskRemainNoneEventHandler : INotificationHandler<UserAskRemainNoneEvent>
    {

        ILogger _logger;

        IServiceProvider _serviceProvider;

        public UserAskRemainNoneEventHandler(
            ILogger<UserAskRemainNoneEventHandler> logger
            , IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }
        public async Task Handle(UserAskRemainNoneEvent notification, CancellationToken cancellationToken)
        {
            Order order = notification.Order;
            using (var scope = _serviceProvider.CreateScope())
            {
                IMessageService messageService = scope.ServiceProvider.GetRequiredService<IMessageService>();

                //结束消息
                SystemStatuMessage systemStatuMessage = new SystemStatuMessage()
                {
                    Content = "您的追问次数已用完"
                };
                Message sendToUser = systemStatuMessage.CreateMessage(default(Guid), order.CreatorID, order.ID, MsgType.System);
                await messageService.SendMessage(sendToUser, systemStatuMessage);


            }
        }
    }
}
