using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PMS.MediatR.Events.PaidQA;
using PMS.MediatR.Request.PaidQA;
using PMS.PaidQA.Application.Services;
using PMS.PaidQA.Domain.Entities;
using PMS.PaidQA.Domain.Enums;
using PMS.PaidQA.Domain.Message;
using PMS.PaidQA.Repository;
using PMS.UserManage.Application.IServices;
using ProductManagement.API.Http.Interface;
using ProductManagement.API.Http.Model.FinanceCenter;
using ProductManagement.Infrastructure.Configs;
using ProductManagement.Tool.Email;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using IMessageService = PMS.PaidQA.Application.Services.IMessageService;
using System.Linq;

namespace PMS.MediatR.Handle.PaidQA
{
    public class TransitingOrderEventHandler : INotificationHandler<TransitingOrderEvent>
    {
        ISxbDarenEmailRepository _darenEmailRepo;
        IMediator _mediator;
        IEmailClient _emailClient;
        ILogger _logger;
        IServiceProvider _serviceProvider;
        PaidQAOption _paidQAOption;
        INotificationService _notificationService;
        public TransitingOrderEventHandler(IServiceProvider serviceProvider, ILogger<OrderOverEventHandler> logger, IMediator mediator, ISxbDarenEmailRepository darenEmailRepo, IEmailClient emailClient, INotificationService notificationService)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _mediator = mediator;
            _emailClient = emailClient;
            _darenEmailRepo = darenEmailRepo;
            _paidQAOption = (serviceProvider.GetService(typeof(IOptions<PaidQAOption>)) as IOptions<PaidQAOption>).Value;
            _notificationService = notificationService;
        }
        public async Task Handle(TransitingOrderEvent notification, CancellationToken cancellationToken)
        {
            Order orginOrder = notification.OrginOrder;

            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    IMessageService messageService = scope.ServiceProvider.GetRequiredService<IMessageService>();
                    IOrderService orderService = scope.ServiceProvider.GetRequiredService<IOrderService>();
                    IFinanceCenterServiceClient financeCenterServiceClient = scope.ServiceProvider.GetRequiredService<IFinanceCenterServiceClient>();
                    IUserService userService = scope.ServiceProvider.GetRequiredService<IUserService>();

                    Order payOrder = new Order();
                    if (notification.RefundAmount > 0)
                    {
                        //发起退款操作（支付中心），取出支付订单
                       var flag =  await orderService.RefundPart(notification.NewOrder, notification.RefundAmount , "转单发起部分退款");
                        if (flag)
                        {
                            //给用户发送退款提醒
                            SystemMessage friendlyMsg = new SystemMessage()
                            {
                                Content = "转单成功！差价已退回您的支付账户，请注意查收！"
                            };
                            Message friendlyMsgToUser = friendlyMsg.CreateMessage(default(Guid), notification.OrginOrder.CreatorID, notification.OrginOrder.ID, MsgType.System);
                            await messageService.SendMessage(friendlyMsgToUser, friendlyMsg);
                        }
                    }
                    //3.4 发起非支付创建订单记录（支付中心）
                    await financeCenterServiceClient.AddPayOrder(new AddPayOrderRequest()
                    {
                        OrderType = ProductManagement.API.Http.Model.FinanceCenter.OrderTypeEnum.Ask,
                        TotalAmount = notification.NewOrder.Amount,
                        UserId = notification.NewOrder.CreatorID,
                        OrderNo = notification.NewOrder.NO,
                        OrderId = notification.NewOrder.ID,
                        TOrderId = notification.OrginOrder.ID,
                        PayAmount = notification.NewOrder.Amount,
                        OrderByProducts = new Orderbyproduct[] {
                           new Orderbyproduct(){
                            ProductId = notification.NewOrder.ID,
                            Remark = "上学问转单",
                            Status = (int)(notification.NewOrder.Status),
                            Amount = notification.NewOrder.Amount
                           }
                           },
                        Remark = "上学问转单",
                        NoNeedPay = 1
                    });


                    //给专家发送用户已转单提醒
                    CustomMessage transtingTips = new CustomMessage()
                    {
                        Content = "您超过12小时未回复用户，用户已选择转单操作，向其他专家发起提问。系统已取消这笔订单并将金额退还给用户。"
                    };
                    await messageService.SendMessage(transtingTips.CreateMessage(default(Guid), notification.OrginOrder.AnswerID, notification.OrginOrder.ID, MsgType.System)
                        , transtingTips
                        );

                    //结束消息
                    SystemStatuMessage overmsg = new SystemStatuMessage()
                    {
                        Content = "本次咨询已结束"
                    };
                    Message sendToUser = overmsg.CreateMessage(default(Guid), orginOrder.CreatorID, orginOrder.ID, MsgType.System);
                    Message sendToTalent = overmsg.CreateMessage(default(Guid), orginOrder.AnswerID, orginOrder.ID, MsgType.System);
                    await messageService.SendMessage(sendToUser, overmsg);
                    await messageService.SendMessage(sendToTalent, overmsg);

                    //发送公众号模板消息
                    var daren = userService.GetUserInfo(notification.OrginOrder.AnswerID);
                    var asker = userService.GetUserInfo(notification.OrginOrder.CreatorID);
                    //发邮件

                    var email_addr = _darenEmailRepo.GetBy($"UserId='{ payOrder.AnswerID}'").FirstOrDefault();
                    if (null != email_addr)
                    {
                        var daren_new = userService.GetUserInfo(payOrder.AnswerID);
                        var link = _paidQAOption.WechatMessageTplSetting.ChangeOrder.link.Replace("{orderId}", payOrder.ID.ToString("N"));
                        var to = new[] { email_addr.Email };

                        var msg = await messageService.GetOrderQuetion(payOrder.ID);//用户转单取哪条消息。待确认
                        await _emailClient.NotifyByMailAsync("您收到一条新到上学问订单（转单",
                            @$"您的专家帐号【{ daren_new?.NickName}】收到了来自用户【{asker.NickName}】的转单提问
                        （原订单编号为【{notification.OrginOrder.NO}】、原订单专家昵称为【{daren.NickName}】）。新订单编号为【{payOrder.NO}】，以下为问题详情内容：【{msg?.Content}】。请及时上线回复用户。详情链接【{link}】。", to);


                    }
                    await _notificationService.NotifiTalentOrderTimeOut(notification.OrginOrder.ID);

                    //更新转单后消息卡片的新订单ID
                    await messageService.UpdateTrantingMsgToHasTranstiy(notification.OrginOrder, notification.NewOrder);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "转单事件处理失败。");
            }
        }
    }
}
