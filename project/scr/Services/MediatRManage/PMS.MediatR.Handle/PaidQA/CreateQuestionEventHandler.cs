using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PMS.MediatR.Events.PaidQA;
using PMS.PaidQA.Application.Services;
using PMS.PaidQA.Domain.Entities;
using PMS.PaidQA.Domain.Enums;
using PMS.PaidQA.Domain.Message;
using PMS.UserManage.Application.IServices;
using ProductManagement.API.Http.Interface;
using ProductManagement.API.Http.Model;
using ProductManagement.API.SMS;
using ProductManagement.Infrastructure.Configs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WeChat.Interface;
using IMessageService = PMS.PaidQA.Application.Services.IMessageService;
using System.Linq;
using System.DrawingCore;
using PMS.MediatR.Handle.PaidQA.EventTask;

namespace PMS.MediatR.Handle.PaidQA
{




    /// <summary>
    /// 处理创建提问成功后事件处理
    /// </summary>
    public class CreateQuestionEventHandler : INotificationHandler<CreateQuestionEvent>
    {
        ILogger _logger;
        IServiceProvider _serviceProvider;
        List<PaidQAEventTask<CreateQuestionEvent>> EventTaks = new List<PaidQAEventTask<CreateQuestionEvent>>();

        public CreateQuestionEventHandler( IServiceProvider serviceProvider
            , ILogger<OrderOverEventHandler> logger
            )
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }


        public async Task Handle(CreateQuestionEvent notification, CancellationToken cancellationToken)
        {
            EventTaks.Add(new UpdateOrderCacheEventTask());
            EventTaks.Add(new SendCreateQuestionNotifyEventTask());
            EventTaks.Add(new SendSystemStatuMessageEventTask());
            foreach (var task in EventTaks)
            {
                try
                {
                    await task.ExcuteAsync(notification,this._serviceProvider);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "创建提问事件处理失败。");
                }
            }

           
        }



        public class UpdateOrderCacheEventTask : PaidQAEventTask<CreateQuestionEvent>
        {
            public override async Task ExcuteAsync(CreateQuestionEvent eventData, IServiceProvider sp)
            {
                using (var scope = sp.CreateScope())
                {
                    IOrderService orderService = scope.ServiceProvider.GetRequiredService<IOrderService>();
                    IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                    await mediator.Publish(new OrderChangeEvent(eventData.Order.ID));

                }
            }
        }

        public class SendSystemStatuMessageEventTask : PaidQAEventTask<CreateQuestionEvent>
        {
            public override async Task ExcuteAsync(CreateQuestionEvent eventData, IServiceProvider sp)
            {
                var order = eventData.Order;

                using (var scope = sp.CreateScope())
                {

                    IMessageService messageService = scope.ServiceProvider.GetRequiredService<IMessageService>();
                    //问题已提交专家
                    SystemStatuMessage systemStatuMessage = new SystemStatuMessage()
                    {
                        Content = "问题已提交专家"
                    };
                    Message sendToUser = systemStatuMessage.CreateMessage(default(Guid), order.CreatorID, order.ID, MsgType.System);
                    await messageService.SendMessage(sendToUser, systemStatuMessage,0,0);

                    //提问完后给用户友好提醒消息
                    SystemMessage friendlyMsg = new SystemMessage()
                    {
                        Content = "专家将会尽快回复您，通常不超过4小时（夜间除外）；若因工作繁忙不能及时回复请耐心等待一下。专家一旦回复，上学帮平台将会通过服务号提醒您，请注意查收消息。"
                    };
                    Message friendlyMsgToUser = friendlyMsg.CreateMessage(default(Guid), order.CreatorID, order.ID, MsgType.System);
                    await messageService.SendMessage(friendlyMsgToUser, friendlyMsg,0,0);


                    //提问完后给专家警告提醒消息
                    SystemMessage warnMsg = new SystemMessage()
                    {
                        Content = "请您尽快回复用户。若您超过12小时还未回复用户，用户可选择进行转单操作（向别的专家发起提问）。若超过24小时未回复用户，则该笔订单将会自动取消，金额将退还给用户。"
                    };
                    Message warnMsgToTalent = warnMsg.CreateMessage(default(Guid), order.AnswerID, order.ID, MsgType.System);
                    await messageService.SendMessage(warnMsgToTalent, warnMsg,0,0);


                }
            }
        }


        public class SendCreateQuestionNotifyEventTask : PaidQAEventTask<CreateQuestionEvent>
        {
            public override async Task ExcuteAsync(CreateQuestionEvent eventData, IServiceProvider sp)
            {
                var notificationService = sp.GetRequiredService<INotificationService>();
                await notificationService.NotifiUserCreateQuestion(eventData.Order.ID);
            }
        }


    }



}
