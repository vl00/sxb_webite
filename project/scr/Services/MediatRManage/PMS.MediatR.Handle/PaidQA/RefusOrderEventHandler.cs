using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PMS.MediatR.Events.PaidQA;
using PMS.MediatR.Handle.PaidQA.EventTask;
using PMS.PaidQA.Application.Services;
using PMS.PaidQA.Domain.Entities;
using PMS.PaidQA.Domain.Enums;
using PMS.PaidQA.Domain.Message;
using PMS.UserManage.Application.IServices;
using ProductManagement.API.Http.Interface;
using ProductManagement.API.Http.Model;
using ProductManagement.API.Http.Model.FinanceCenter;
using ProductManagement.Infrastructure.Configs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WeChat;
using IMessageService = PMS.PaidQA.Application.Services.IMessageService;

namespace PMS.MediatR.Handle.PaidQA
{
    /// <summary>
    /// 拒绝订单时所需的处理
    /// </summary>
    public class RefusOrderEventHandler : INotificationHandler<RefusOrderEvent>
    {
        IServiceProvider _serviceProvider;
        ILogger _logger;
        CustomMsgSetting _customMsgSetting;
        ICouponTakeService _couponTakeService;
        List<PaidQAEventTask<RefusOrderEvent>> EventTasks = new List<PaidQAEventTask<RefusOrderEvent>>();

        public RefusOrderEventHandler(IServiceProvider serviceProvider, ILogger<RefusOrderEventHandler> logger
            , IOptions<PaidQAOption> options, ICouponTakeService couponTakeService)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _customMsgSetting = options.Value.CustomMsgSetting;
            _couponTakeService = couponTakeService;
        }

        public async Task Handle(RefusOrderEvent notification, CancellationToken cancellationToken)
        {
            
            EventTasks.Add(new SendCustomMessageTask());
            EventTasks.Add(new RefundTask());
            EventTasks.Add(new BackCouponTask());
            EventTasks.Add(new NotifyUserHasRefusTask());
            
            foreach (var task in EventTasks)
            {
                try
                {
                    await task.ExcuteAsync(notification, _serviceProvider);
                }
                catch (Exception ex){
                    _logger.LogError(ex, $"处理拒绝订单事件异常。TaskName={task.GetType().FullName}");
                }
            }
        }

        public class SendCustomMessageTask : PaidQAEventTask<RefusOrderEvent>
        {
            public override async Task ExcuteAsync(RefusOrderEvent eventData, IServiceProvider sp)
            {
                Order order = eventData.Order;
                IMessageService messageService = sp.GetRequiredService<IMessageService>();

                //发送客服消息给专家
                CustomMessage customMessageTalent = new CustomMessage() { Content = "您已拒绝该用户的提问订单，系统已取消这笔订单并将金额退还给用户。" };
                await messageService.SendMessage(customMessageTalent.CreateMessage(default(Guid), order.AnswerID, order.ID, MsgType.System)
                    , customMessageTalent
                    );
                //发送客服消息给用户
                CustomMessage customMessageUser = new CustomMessage() { Content = "由于专家的个人原因，暂时没有办法回答您的这个提问，十分抱歉，请您谅解，谢谢！" };
                await messageService.SendMessage(customMessageUser.CreateMessage(default(Guid), order.CreatorID, order.ID, MsgType.System)
                    , customMessageUser
                    );

                //结束消息
                SystemStatuMessage overMsg = new SystemStatuMessage()
                {
                    Content = "本次咨询已结束"
                };
                Message sendToUser = overMsg.CreateMessage(default(Guid), order.CreatorID, order.ID, MsgType.System);
                Message sendToTalent = overMsg.CreateMessage(default(Guid), order.AnswerID, order.ID, MsgType.System);
                await messageService.SendMessage(sendToUser, overMsg);
                await messageService.SendMessage(sendToTalent, overMsg);
            }
        }

        public class RefundTask : PaidQAEventTask<RefusOrderEvent>
        {
            public override async Task ExcuteAsync(RefusOrderEvent eventData, IServiceProvider sp)
            {
                IOrderService orderService = sp.GetRequiredService<IOrderService>();
                //退款
                await orderService.Refund(eventData.Order, "专家拒绝订单发起退款");

            }
        }
        public class BackCouponTask : PaidQAEventTask<RefusOrderEvent>
        {
            public override async Task ExcuteAsync(RefusOrderEvent eventData, IServiceProvider sp)
            {
                ICouponTakeService  couponTakeService = sp.GetRequiredService<ICouponTakeService>();
                //退券
                await couponTakeService.BackCoupon(eventData.Order.ID);

            }
        }

        public class NotifyUserHasRefusTask : PaidQAEventTask<RefusOrderEvent>
        {
            public override async Task ExcuteAsync(RefusOrderEvent eventData, IServiceProvider sp)
            {
                INotificationService notificationService = sp.GetRequiredService<INotificationService>();
                await notificationService.NotifiUserRefusOrder(eventData.Order.ID);

            }
        }
    }
}
