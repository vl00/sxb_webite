using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PMS.MediatR.Events.PaidQA;
using PMS.MediatR.Handle.PaidQA.EventTask;
using PMS.MediatR.Request.PaidQA;
using PMS.PaidQA.Application.Services;
using PMS.PaidQA.Domain.Entities;
using PMS.SignalR;
using PMS.SignalR.Clients.PaidQAClient;
using PMS.SignalR.Clients.PaidQAClient.Models;
using PMS.SignalR.Hubs.PaidQAHub;
using PMS.UserManage.Application.IServices;
using ProductManagement.Framework.Cache.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using IMessageService = PMS.PaidQA.Application.Services.IMessageService;

namespace PMS.MediatR.Handle.PaidQA
{


    /// <summary>
    /// 订单状态变更
    /// </summary>
    public class OrderChangeEventHandler : INotificationHandler<OrderChangeEvent>
    {

        ILogger _logger;

        IServiceProvider _serviceProvider;

        List<PaidQAEventTask<OrderChangeEvent>> eventTasks = new List<PaidQAEventTask<OrderChangeEvent>>();

        public OrderChangeEventHandler(
            ILogger<OrderChangeEventHandler> logger
            , IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }


        public async Task Handle(OrderChangeEvent notification, CancellationToken cancellationToken)
        {
            eventTasks.Add(new SignalRTask());
            try
            {
                foreach (var task in eventTasks)
                {
                    await task.ExcuteAsync(notification, _serviceProvider);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "处理订单变更事件失败。");
            }
        }

        public class SignalRTask : PaidQAEventTask<OrderChangeEvent>
        {
            public override async Task ExcuteAsync(OrderChangeEvent eventData, IServiceProvider sp)
            {
                using (var scope = sp.CreateScope())
                {
                    IOrderService _orderService = scope.ServiceProvider.GetService<IOrderService>();
                    await _orderService.SetToCache(eventData.OrderID);

                    Order order = await _orderService.GetAsync(eventData.OrderID);
                    IMapper mapper = scope.ServiceProvider.GetService<IMapper>();
                    IMediator mediator = scope.ServiceProvider.GetService<IMediator>();
                    IEvaluateService evaluateService = scope.ServiceProvider.GetService<IEvaluateService>();
                    IEvaluateTagsService evaluateTagsService = scope.ServiceProvider.GetService<IEvaluateTagsService>();
                    IUserService userService = scope.ServiceProvider.GetService<IUserService>();
                    IMessageService messageService = scope.ServiceProvider.GetService<IMessageService>();
                    var orderChatHubContext = scope.ServiceProvider.GetService<IHubContext<OrderChatHub, IOrderChatClient>>();

                    //if (_orderService.CheckOrderIsOverTime(order))
                    //{
                    //    //触发订单超时事件
                    //    await mediator.Publish(new OverTimeOrderEvent(order));
                    //    //更新一下当前订单
                    //    order = await _orderService.GetFromCache(order.ID);
                    //}
                    ReceiveOrderChangeResult result = new ReceiveOrderChangeResult();
                    result.OrderInfo = mapper.Map<OrderInfoResult>(order);
                    if (order.IsEvaluate)
                    {
                        //获取评价信息
                        var evalute = (await evaluateService.GetByOrderIDs(new List<Guid>() { order.ID })).FirstOrDefault();
                        result.EvaluateInfo = mapper.Map<EvaluateResult>(evalute);
                        var evaluteTags = await evaluateTagsService.GetByEvaluateIDs(new List<Guid> { evalute.ID });
                        if (evaluteTags != null && evaluteTags.Any())
                        {
                            result.EvaluateInfo.Tags = evaluteTags.First().Value?.Select(t => t.Name).ToList();
                        }

                    }


                    if (await messageService.IsOnline(order.ID, order.CreatorID))
                    {
                        string creatorGroupName = OrderChatHubHelper.OrderUserGroupName(order.ID, order.CreatorID);
                        await orderChatHubContext.Clients.Group(creatorGroupName).ReceiveOrderChange(result);
                    }
                    if (await messageService.IsOnline(order.ID, order.AnswerID))
                    {
                        string answerGroupName = OrderChatHubHelper.OrderUserGroupName(order.ID, order.AnswerID);
                        await orderChatHubContext.Clients.Group(answerGroupName).ReceiveOrderChange(result);
                    }


                }


            }
        }



    }
}
