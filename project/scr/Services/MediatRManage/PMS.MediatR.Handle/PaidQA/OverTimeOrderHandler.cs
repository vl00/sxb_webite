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
using Microsoft.Extensions.Logging;
using System.Linq;
using ProductManagement.API.Http.Interface;
using ProductManagement.API.Http.Model.FinanceCenter;
using PMS.PaidQA.Domain.Enums;
using WeChat;
using PMS.UserManage.Application.IServices;
using IMessageService = PMS.PaidQA.Application.Services.IMessageService;
using ProductManagement.API.Http.Model;
using ProductManagement.Infrastructure.Configs;
using Microsoft.Extensions.Options;
using PMS.MediatR.Request.PaidQA;
using OrderStatus = PMS.PaidQA.Domain.Enums.OrderStatus;
using PMS.MediatR.Handle.PaidQA.EventTask;

namespace PMS.MediatR.Handle.PaidQA
{
    /// <summary>
    /// 处理超时订单
    /// </summary>
    public class OverTimeOrderHandler : INotificationHandler<OverTimeOrderEvent>
    {
        ILogger _logger;
        IServiceProvider _serviceProvider;
        List<PaidQAEventTask<OverTimeOrderEvent>> TimeOutEventTasks = new List<PaidQAEventTask<OverTimeOrderEvent>>();
        public OverTimeOrderHandler(IServiceProvider serviceProvider, ILogger<OverTimeOrderHandler> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task Handle(OverTimeOrderEvent notification, CancellationToken cancellationToken)
        {
            //超时订单有两种处理，如果状态是进行中的，走正常结束流程。否则，标记为已超时并且退款
            TimeOutEventTasks.Add(new NotifyUserOrderTimeOutTask());
            TimeOutEventTasks.Add(new NotifyTalentOrderTimeOutTask());
            foreach (var task in TimeOutEventTasks)
            {
                try
                {
                    await task.ExcuteAsync(notification, _serviceProvider);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"超时订单异常结束处理异常，订单ID={notification.Order.ID} TaksName={task.GetType().FullName}");
                }

            }

        }


        public class NotifyUserOrderTimeOutTask : PaidQAEventTask<OverTimeOrderEvent>
        {
            public override async Task ExcuteAsync(OverTimeOrderEvent eventData, IServiceProvider sp)
            {
                INotificationService notificationService = sp.GetRequiredService<INotificationService>();
                await notificationService.NotifiUserOrderTimeOut(eventData.Order.ID);
            }
        }
        public class NotifyTalentOrderTimeOutTask : PaidQAEventTask<OverTimeOrderEvent>
        {
            public override async Task ExcuteAsync(OverTimeOrderEvent eventData, IServiceProvider sp)
            {
                INotificationService notificationService = sp.GetRequiredService<INotificationService>();
                await notificationService.NotifiTalentOrderTimeOut(eventData.Order.ID);
            }
        }

    }
}
