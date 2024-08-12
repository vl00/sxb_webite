
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PMS.MediatR.Events.PaidQA;
using PMS.PaidQA.Application.Services;
using PMS.PaidQA.Domain.Entities;
using PMS.PaidQA.Domain.Enums;
using PMS.PaidQA.Domain.Message;
using ProductManagement.API.SMS;
using ProductManagement.API.SMS.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using ProductManagement.Infrastructure.Configs;
using Microsoft.Extensions.Options;
using PMS.UserManage.Application.IServices;
using PMS.MediatR.Handle.PaidQA.EventTask;
using IMessageService = PMS.PaidQA.Application.Services.IMessageService;

namespace PMS.MediatR.Handle.PaidQA
{
    public class OrderRefundEventHandler : INotificationHandler<OrderRefundEvent>
    {
        ILogger _logger;

        IServiceProvider _serviceProvider;

        List<PaidQAEventTask<OrderRefundEvent>> EventTasks = new List<PaidQAEventTask<OrderRefundEvent>>();

        public OrderRefundEventHandler(IServiceProvider serviceProvider, ILogger<OverTimeOrderHandler> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }


        public async Task Handle(OrderRefundEvent notification, CancellationToken cancellationToken)
        {
            EventTasks.Add(new SendCustomMessageEventTask());
            EventTasks.Add(new NotifyRefundEventTask());
            foreach (var task in EventTasks)
            {
                try
                {
                    await task.ExcuteAsync(notification, _serviceProvider);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "处理订单退款成功事件异常。");
                }

            }


        }


        public class SendCustomMessageEventTask : PaidQAEventTask<OrderRefundEvent>
        {
            public override async Task ExcuteAsync(OrderRefundEvent eventData, IServiceProvider sp)
            {
                IMessageService messageService = sp.GetRequiredService<IMessageService>();
                //提问完后给用户友好提醒消息
                SystemMessage refundTips = new SystemMessage()
                {
                    Content = "未能解决您的疑惑我们感到十分抱歉。您支付的金额已退还至您的账户，请注意查收！"
                };

                Message friendlyMsgToUser = refundTips.CreateMessage(default(Guid), eventData.Order.CreatorID, eventData.Order.ID, MsgType.System);
                await messageService.SendMessage(friendlyMsgToUser, refundTips);

            }
        }
        public class NotifyRefundEventTask : PaidQAEventTask<OrderRefundEvent>
        {
            public override async Task ExcuteAsync(OrderRefundEvent eventData, IServiceProvider sp)
            {

                INotificationService notificationService = sp.GetRequiredService<INotificationService>();
                await notificationService.NotifiUserRefundAmount(eventData.Order.ID);
            }
        }
    }
}
