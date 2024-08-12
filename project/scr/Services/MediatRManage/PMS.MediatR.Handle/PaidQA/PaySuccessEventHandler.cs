using MediatR;
using PMS.MediatR.Events.PaidQA;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using PMS.PaidQA.Application.Services;
using PMS.PaidQA.Domain.Message;
using PMS.PaidQA.Domain.Entities;
using PMS.PaidQA.Domain.Enums;

namespace PMS.MediatR.Handle.PaidQA
{
    public class PaySuccessEventHandler : INotificationHandler<PaySuccessEvent>
    {
        IServiceProvider _serviceProvider;
        public PaySuccessEventHandler(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public async Task Handle(PaySuccessEvent notification, CancellationToken cancellationToken)
        {
            //发送本次咨询已开始信息
            using (var scope = _serviceProvider.CreateScope())
            {
                IMessageService messageService = scope.ServiceProvider.GetRequiredService<IMessageService>();
                IOrderService orderService = scope.ServiceProvider.GetRequiredService<IOrderService>();
                Order order = orderService.Get(notification.OrderID);

                //订单编号
                SystemStatuMessage orderNoMsg = new SystemStatuMessage()
                {
                    Content = $"订单编号：{order.NO}"
                };
                Message orderNoMsgsendToUser = orderNoMsg.CreateMessage(default(Guid), order.CreatorID, order.ID, MsgType.System);
                Message orderNoMsgsendToTalent = orderNoMsg.CreateMessage(default(Guid), order.AnswerID, order.ID, MsgType.System);
                await messageService.SendMessage(orderNoMsgsendToUser, orderNoMsg);
                await messageService.SendMessage(orderNoMsgsendToTalent, orderNoMsg);
                //本次咨询开始
                SystemStatuMessage systemStatuMessage = new SystemStatuMessage() {
                    Content = "本次咨询开始",
                };
                Message sendToUser = systemStatuMessage.CreateMessage(default(Guid), order.CreatorID, order.ID, MsgType.System);
                Message sendToTalent = systemStatuMessage.CreateMessage(default(Guid), order.AnswerID, order.ID,MsgType.System);
                await messageService.SendMessage(sendToUser, systemStatuMessage);
                await messageService.SendMessage(sendToTalent, systemStatuMessage);

            }

        }
    }
}
