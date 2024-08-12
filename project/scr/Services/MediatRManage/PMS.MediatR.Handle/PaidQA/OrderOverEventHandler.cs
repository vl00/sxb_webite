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
using ProductManagement.API.SMS;
using ProductManagement.Infrastructure.Configs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WeChat;
using IMessageService = PMS.PaidQA.Application.Services.IMessageService;

namespace PMS.MediatR.Handle.PaidQA
{

    /// <summary>
    /// 订单正常结束事件处理
    /// </summary>
    public class OrderOverEventHandler : INotificationHandler<OrderOverEvent>
    {
        ILogger _logger;
        IServiceProvider _serviceProvider;
        IFinanceCenterServiceClient _financeCenterServiceClient;
        CustomMsgSetting _customMsgSetting;
        List<PaidQAEventTask<OrderOverEvent>> EventTaks = new List<PaidQAEventTask<OrderOverEvent>>();

        public OrderOverEventHandler(IServiceProvider serviceProvider, ILogger<OrderOverEventHandler> logger, IFinanceCenterServiceClient financeCenterServiceClient
            , IOptions<PaidQAOption> options)
        {
            
            _financeCenterServiceClient = financeCenterServiceClient;
            _serviceProvider = serviceProvider;
            _logger = logger;
            _customMsgSetting = options.Value.CustomMsgSetting;

        }
        public async Task Handle(OrderOverEvent notification, CancellationToken cancellationToken)
        {
            EventTaks.Add(new FWHOrSMSNotify());
            EventTaks.Add(new SendOverNotify());
            foreach (var task in EventTaks)
            {
                try
                {
                   await  task.ExcuteAsync(notification, _serviceProvider);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "订单结束事件处理失败");
                }
            }


        }

        public class SendOverNotify : PaidQAEventTask<OrderOverEvent>
        {
            public override async Task ExcuteAsync(OrderOverEvent eventData, IServiceProvider sp)
            {
                using (var scope = sp.CreateScope())
                {
                    IMessageService messageService = scope.ServiceProvider.GetRequiredService<IMessageService>();
                    IWeChatAppClient weChatAppClient = scope.ServiceProvider.GetRequiredService<IWeChatAppClient>();
                    IUserService userService = scope.ServiceProvider.GetRequiredService<IUserService>();
                    //结束消息
                    SystemStatuMessage systemStatuMessage = new SystemStatuMessage()
                    {
                        Content = "本次咨询已结束"
                    };
                    Message sendToUser = systemStatuMessage.CreateMessage(default(Guid), eventData.Order.CreatorID, eventData.Order.ID, MsgType.System);
                    Message sendToTalent = systemStatuMessage.CreateMessage(default(Guid), eventData.Order.AnswerID, eventData.Order.ID, MsgType.System);
                    await messageService.SendMessage(sendToUser, systemStatuMessage);
                    await messageService.SendMessage(sendToTalent, systemStatuMessage);
                    //专家邀请好评消息
                    TextMessage inviteEvaluteMessage = new TextMessage()
                    {
                        Content = "感谢您对我的信任，如果您觉得我的回答能对您有所帮助的话，请给我一个五星好评吧，谢谢～"
                    };
                    Message inviteMsgSendToUser = inviteEvaluteMessage.CreateMessage(eventData.Order.AnswerID, eventData.Order.CreatorID, eventData.Order.ID, MsgType.Answer);
                    await messageService.SendMessage(inviteMsgSendToUser, inviteEvaluteMessage);
                    //伪装专家来发送消息要手动地给专家的未读消息队列推一下。
                    await messageService.PushUnReadMsg(eventData.Order.ID, inviteMsgSendToUser.SenderID.Value, inviteMsgSendToUser);
                    //发给专家一个等待评价系统消息
                    SystemStatuMessage waitEvaluteMsg = new SystemStatuMessage()
                    {
                        Content = "等待用户评价"
                    };
                    Message waitEvaluteMsSendToTalent = waitEvaluteMsg.CreateMessage(default(Guid), eventData.Order.AnswerID, eventData.Order.ID, MsgType.System);
                    await messageService.SendMessage(waitEvaluteMsSendToTalent, waitEvaluteMsg);


                }

            }
        }


        public class FWHOrSMSNotify : PaidQAEventTask<OrderOverEvent>
        {
            public override async Task ExcuteAsync(OrderOverEvent eventData, IServiceProvider sp)
            {
                var notificationService = sp.GetRequiredService<INotificationService>();
                await notificationService.NotifiUserEvalute(eventData.Order.ID);
            }
        }

    }
}
