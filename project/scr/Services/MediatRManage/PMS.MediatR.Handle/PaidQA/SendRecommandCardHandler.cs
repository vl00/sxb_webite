using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PMS.MediatR.Events.PaidQA;
using PMS.PaidQA.Application.Services;
using PMS.PaidQA.Domain.Entities;
using PMS.PaidQA.Domain.Enums;
using PMS.PaidQA.Domain.Message;
using ProductManagement.Framework.Cache.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ProductManagement.Framework.Foundation;
using WeChat;
using ProductManagement.API.Http.Interface;
using ProductManagement.API.Http.Model;
using PMS.UserManage.Application.IServices;
using IMessageService = PMS.PaidQA.Application.Services.IMessageService;
using ProductManagement.Infrastructure.Configs;
using Microsoft.Extensions.Options;
using PMS.MediatR.Handle.PaidQA.EventTask;

namespace PMS.MediatR.Handle.PaidQA
{
    /// <summary>
    /// 处理12小时转单提醒
    /// </summary>
    public class SendRecommandCardHandler : INotificationHandler<SendRecommandCardEvent>
    {
        ILogger _logger;
        IServiceProvider _serviceProvider;

        List<PaidQAEventTask<SendRecommandCardEvent>> EventTaks = new List<PaidQAEventTask<SendRecommandCardEvent>>();
        public SendRecommandCardHandler(
            ILogger<SendRecommandCardHandler> logger
            , IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }


        public async Task Handle(SendRecommandCardEvent notification, CancellationToken cancellationToken)
        {
            EventTaks.Add(new SendRecommandCardMsg());
            EventTaks.Add(new SendTranstingNotify());
            foreach (var task in EventTaks)
            {
                try
                {
                    await task.ExcuteAsync(notification, _serviceProvider);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "订单结束事件处理失败");
                }
            }



        }
        public class SendRecommandCardMsg : PaidQAEventTask<SendRecommandCardEvent>
        {
            public override async Task ExcuteAsync(SendRecommandCardEvent eventData, IServiceProvider sp)
            {
                using (var scope = sp.CreateScope())
                {
                    IOrderService _orderService = scope.ServiceProvider.GetRequiredService<IOrderService>();
                    IMessageService _messageService = scope.ServiceProvider.GetRequiredService<IMessageService>();
                    ITalentSettingService _talentSettingService = scope.ServiceProvider.GetRequiredService<ITalentSettingService>();
                    IWeChatAppClient weChatAppClient = scope.ServiceProvider.GetRequiredService<IWeChatAppClient>();
                    IUserService userService = scope.ServiceProvider.GetRequiredService<IUserService>();
                    ICouponTakeService _couponTakeService = scope.ServiceProvider.GetRequiredService<ICouponTakeService>();
                    if (await _couponTakeService.OrderHasUseCoupon(eventData.Order.ID))
                    {
                        //使用优惠券的订单屏蔽转单入口
                        return;
                    }

                    var finds = await _talentSettingService.GetSimilarTalents(eventData.Order.AnswerID, 1);
                    var targetTalent = finds.FirstOrDefault();
                    //发送推荐消息
                    var recommandCard = new RecommandCardMessage()
                    {
                        Content = "检测到您的订单已超过12小时还未收到专家的回复，已匹配到能为您解决问题的专家，是否需要为您转单？",
                        Status = RecommandCardMessageStatu.Default,
                        TalentUserID = targetTalent.TalentUserID,
                        PriceSpread = (eventData.Order.Amount - targetTalent.Price).CutDecimalWithN(2).ToString()
                    };
                    var sendResult = await _messageService.SendMessage(recommandCard.CreateMessage(default(Guid), eventData.Order.CreatorID, eventData.Order.ID, MsgType.System)
                        , recommandCard);
                }

            }
        }

        public class SendTranstingNotify : PaidQAEventTask<SendRecommandCardEvent>
        {
            public override async Task ExcuteAsync(SendRecommandCardEvent eventData, IServiceProvider sp)
            {
                using (var scope = sp.CreateScope())
                {
                    var notificationService = sp.GetRequiredService<INotificationService>();
                    await notificationService.NotifiTransting(eventData.Order.ID);
                }

            }
        }



    }
}
