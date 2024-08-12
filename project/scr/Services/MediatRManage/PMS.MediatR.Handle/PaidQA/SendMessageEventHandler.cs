using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PMS.MediatR.Events.PaidQA;
using PMS.MediatR.Request.PaidQA;
using PMS.PaidQA.Application.Services;
using PMS.PaidQA.Domain.Entities;
using PMS.PaidQA.Domain.Enums;
using PMS.PaidQA.Domain.Message;
using PMS.PaidQA.Repository;
using PMS.SignalR;
using PMS.SignalR.Clients.PaidQAClient;
using PMS.SignalR.Hubs.PaidQAHub;
using PMS.UserManage.Application.IServices;
using ProductManagement.Infrastructure.Configs;
using ProductManagement.Tool.Email;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using IMessageService = PMS.PaidQA.Application.Services.IMessageService;
using System.Linq;
using Microsoft.Extensions.Options;
using PMS.MediatR.Handle.PaidQA.EventTask;
using PMS.SignalR.Clients.PaidQAClient.Models;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using PMS.AspNetCoreHelper;

namespace PMS.MediatR.Handle.PaidQA
{
    public class SendMessageEventHandler : INotificationHandler<SendMessageEvent>
    {

        ILogger _logger;
        IServiceProvider _serviceProvider;
        List<PaidQAEventTask<SendMessageEvent>> eventTasks = new List<PaidQAEventTask<SendMessageEvent>>();

        public SendMessageEventHandler(IServiceProvider serviceProvider, ILogger<SendMessageEventHandler> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task Handle(SendMessageEvent notification, CancellationToken cancellationToken)
        {
            eventTasks.Add(new InvokeSignalRTask());
            eventTasks.Add(new SendFwhTemplateMessageTask());

            foreach (var task in eventTasks)
            {
                try
                {
                    await task.ExcuteAsync(notification, _serviceProvider);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"发送消息事件异常;messageId={notification.Message.ID}");
                }
            }

        }


        public class SendFwhTemplateMessageTask : PaidQAEventTask<SendMessageEvent>
        {
            public override async Task ExcuteAsync(SendMessageEvent eventData, IServiceProvider sp)
            {
                using (var scope = sp.CreateScope())
                {
                    var message = eventData.Message;
                    var order = eventData.Order;
                    IMediator _mediator = scope.ServiceProvider.GetService<IMediator>();
                    ISxbDarenEmailRepository _darenEmailRepo = scope.ServiceProvider.GetService<ISxbDarenEmailRepository>();
                    PaidQAOption _paidQAOption = scope.ServiceProvider.GetService<IOptions<PaidQAOption>>().Value;
                    IEmailClient _emailClient = scope.ServiceProvider.GetService<IEmailClient>();
                    IUserService userService = scope.ServiceProvider.GetRequiredService<IUserService>();
                    IOrderService orderService = scope.ServiceProvider.GetRequiredService<IOrderService>();

                    //更新订单缓存
                    //await orderService.SetToCache(order.ID);
                    await _mediator.Publish(new OrderChangeEvent(order.ID));


                    if (message.SenderID == default(Guid) || message.ReceiveID == default(Guid))
                    {
                        //系统消息不需要处理
                        return;
                    }
                    order = orderService.Get(order.ID);
                    var senderInfo = userService.GetUserInfo(message.SenderID.GetValueOrDefault());
                    var receiverInfo = userService.GetUserInfo(message.ReceiveID.GetValueOrDefault());
                    string msgContent = PaidQAMessage.GetContent(message);
                    if (!string.IsNullOrEmpty(msgContent) && msgContent.Length > 30)
                    {
                        msgContent = msgContent.Substring(0, 30) + "...";
                    }


                    if (message.MsgType == PMS.PaidQA.Domain.Enums.MsgType.Question || message.MsgType == PMS.PaidQA.Domain.Enums.MsgType.GoAsk)
                    {
                        if (order.IsAnonymous == true) { senderInfo.NickName = order.AnonyName; }
                        var daren = userService.GetUserInfo(message.ReceiveID.GetValueOrDefault());
                        int whatTimeNow = orderService.GetHasAskCount(order) - 1;
                        if (orderService.IsFirstAsk(order))//首次提问
                        {

                            //发邮件
                            var email_addr = _darenEmailRepo.GetBy($"UserId='{ message.ReceiveID}'").FirstOrDefault();
                            if (null != email_addr)
                            {
                                var link = _paidQAOption.WechatMessageTplSetting.PayAskRecieve.link.Replace("{orderId}", order.ID.ToString("N"));
                                var to = new[] { email_addr.Email };
                                await _emailClient.NotifyByMailAsync("您收到一条新到上学问订单", $@"
您的专家帐号【{ daren?.NickName}】收到了来自用户【{senderInfo.NickName}】的新提问，订单编号为{order.NO}，以下为问题详情内容：{message.Content}。请及时上线回复用户。详情链接{link}。", to);

                            }

                        }
                        else
                        {

                            //发邮件
                            var email_addr = _darenEmailRepo.GetBy($"UserId='{ message.ReceiveID}'").FirstOrDefault();
                            if (null != email_addr)
                            {
                                var link = _paidQAOption.WechatMessageTplSetting.AddAsk.link.Replace("{orderId}", order.ID.ToString("N"));
                                var to = new[] { email_addr.Email };

                                await _emailClient.NotifyByMailAsync("您的上学问订单有追问消息", $@"您的专家帐号【{ daren?.NickName}】收到了来自用户【{senderInfo.NickName}】的追问，
订单编号为{order.NO}，以下为追问内容{message.Content}。请及时上线回复用户。详情链接{link}。", to);



                            }
                        }
                    }


                    if (!userService.TryGetOpenId(message.ReceiveID.GetValueOrDefault(), "fwh", out string reciverOpenID)) return;
                    //专家回复提醒用户
                    if (message.MsgType == PMS.PaidQA.Domain.Enums.MsgType.Answer)
                    {
                        var cmd = new AskWechatTemplateSendRequest()
                        {
                            user_nickname = receiverInfo?.NickName,
                            keyword1 = senderInfo.NickName,
                            keyword2 = TimeFormart(message.CreateTime.GetValueOrDefault()),
                            keyword3 = msgContent,
                            openid = reciverOpenID,
                            remark = "点击【查看详情】看专家回复",
                            msgtype = WechatMessageType.专家回复问题,
                            OrderID = order.ID
                        };
                        var wechatMsgResult = await _mediator.Send(cmd);

                    }//专家收到提问&&追问&&最后一次提问
                    else if (message.MsgType == PMS.PaidQA.Domain.Enums.MsgType.Question || message.MsgType == PMS.PaidQA.Domain.Enums.MsgType.GoAsk)
                    {
                        if (order.IsAnonymous == true) { senderInfo.NickName = order.AnonyName; }
                        var daren = userService.GetUserInfo(message.ReceiveID.GetValueOrDefault());
                        int whatTimeNow = orderService.GetHasAskCount(order) - 1;
                        if (orderService.IsFirstAsk(order))//首次提问
                        {
                            var cmd = new AskWechatTemplateSendRequest()
                            {
                                daren_nickname = daren?.NickName,
                                keyword1 = senderInfo.NickName,
                                keyword2 = $"该咨询将于{TimeFormart(order.CreateTime.AddHours(24))}失效",
                                keyword3 = "上学问",
                                keyword4 = TimeFormart(message.CreateTime.GetValueOrDefault()),
                                openid = reciverOpenID,
                                remark = "点击【查看详情】马上回复",
                                msgtype = WechatMessageType.收到提问,
                                OrderID = order.ID
                            };
                            var wechatMsgResult = await _mediator.Send(cmd);


                        }
                        else if (orderService.IsLastAsk(order))
                        {
                            // 最后一次追问
                            var cmdAddAsk = new AskWechatTemplateSendRequest()
                            {
                                daren_nickname = daren?.NickName,
                                keyword1 = senderInfo.NickName,
                                keyword2 = $"第{whatTimeNow}次追问",//追问次数,
                                keyword3 = "上学问",
                                keyword4 = TimeFormart(message.CreateTime.GetValueOrDefault()),
                                openid = reciverOpenID,
                                remark = "点击【查看详情】马上回复",
                                msgtype = WechatMessageType.用户发起最后一次追问,
                                OrderID = order.ID
                            };
                            var addAskWechatMsgResult = await _mediator.Send(cmdAddAsk);

                        }
                        else
                        {
                            var cmdAddAsk = new AskWechatTemplateSendRequest()
                            {
                                daren_nickname = daren?.NickName,
                                keyword1 = senderInfo.NickName,
                                keyword2 = $"第{whatTimeNow}次追问",//追问次数,
                                keyword3 = "上学问",
                                keyword4 = TimeFormart(message.CreateTime.GetValueOrDefault()),
                                openid = reciverOpenID,
                                remark = "点击【查看详情】马上回复",
                                msgtype = WechatMessageType.用户发起追问,
                                OrderID = order.ID
                            };
                            var addAskWechatMsgResult = await _mediator.Send(cmdAddAsk);

                        }
                    }

                }
            }

            string TimeFormart(DateTime dateTime)
            {
                return dateTime.ToString("yyyy年MM月dd日 HH:mm:ss");
            }
        }

        public class InvokeSignalRTask : PaidQAEventTask<SendMessageEvent>
        {
            string defaultHeadImg = "https://cos.sxkid.com/images/head.png";
            public override async Task ExcuteAsync(SendMessageEvent eventData, IServiceProvider sp)
            {

                //以系统角色发时，需要广播到两端用户。(这里有一种特殊情况需要处理，系统可以模拟用户发送，所以需要特许判断该情况)
                if (eventData.SendMessageOrigin == SendMessageOrigin.System)
                {
                    var message = eventData.Message;
                    var order = eventData.Order;
                    using (var scope = sp.CreateScope())
                    {
                        IMapper mapper = scope.ServiceProvider.GetService<IMapper>();
                        IUserService userService = scope.ServiceProvider.GetService<IUserService>();
                        IMessageService messageService = scope.ServiceProvider.GetService<IMessageService>();
                        var orderChatHubContext = scope.ServiceProvider.GetService<IHubContext<OrderChatHub, IOrderChatClient>>();
                        var userInfo = userService.GetUserInfo(message.SenderID.GetValueOrDefault());
                        MessageResult messageResult = mapper.Map<MessageResult>(message);
                        List<MessageResult> messageResults = new List<MessageResult>() { messageResult };


                        if (message.SenderID != null && Guid.Empty != message.SenderID)
                        {
                            //发给发送者。(这是特殊的模拟用户发送情况。)
                            List<MessageResult>  newMessageResults = MessageAddSenderInfo(userService, mapper, messageResults, order, messageResult.SenderID.Value);
                            await orderChatHubContext.Clients.Group(OrderChatHubHelper.OrderUserGroupName(order.ID, message.SenderID.Value)).ReceiveMessage(new ReceiveMessageResult()
                            {
                                Messages = newMessageResults
                            });
                        }
                        if (await messageService.IsOnline(order.ID, messageResult.ReceiveID.Value))
                        {
                            //发给接收者
                            messageResults = MessageAddSenderInfo(userService, mapper, messageResults, order, messageResult.ReceiveID.Value);
                            await orderChatHubContext.Clients.Group(OrderChatHubHelper.OrderUserGroupName(order.ID, message.ReceiveID.Value)).ReceiveMessage(new ReceiveMessageResult()
                            {
                                Messages = messageResults
                            });
                            //更新消息为已读
                            await messageService.UpdateMessageReadTime(message.ID);
                        }
               

                    }
                }

            }


            /// <summary>
            /// 赋值消息的SenderInfo字段
            /// </summary>
            /// <param name="messages"></param>
            /// <param name="order"></param>
            List<MessageResult> MessageAddSenderInfo(IUserService userService, IMapper mapper, List<MessageResult> messages, Order order, Guid reciverId)
            {
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(messages);
                List<MessageResult> newMessages = Newtonsoft.Json.JsonConvert.DeserializeObject<List<MessageResult>>(json);
                newMessages.ForEach(msg =>
               {
                   msg.IsBelongSelf = msg.SenderID == reciverId;
                   var userInfo = userService.GetUserInfo(msg.SenderID.Value);
                   msg.SenderInfo = mapper.Map<UserInfoResult>(userInfo);
                   if (reciverId == order.AnswerID
                      && order.IsAnonymous == true
                      && msg.SenderID == order.CreatorID)
                   {
                        //只有当前登录者是专家身份，并且消息是用户发的才需要屏蔽，专家消息不处理
                        msg.SenderInfo.NickName = order.AnonyName;
                       msg.SenderInfo.HeadImgUrl = defaultHeadImg;
                   }

               });
                return newMessages;
            }
        }



    }
}
