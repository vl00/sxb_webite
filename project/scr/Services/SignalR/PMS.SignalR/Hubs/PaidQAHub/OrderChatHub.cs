using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using PMS.SignalR.Clients.PaidQAClient;
using System;
using System.Threading.Tasks;
using PMS.PaidQA.Domain.Entities;
using Microsoft.AspNetCore.Http;
using PMS.PaidQA.Application.Services;
using PMS.AspNetCoreHelper;
using PMS.SignalR.Clients.PaidQAClient.Models;
using System.Collections.Generic;
using ProductManagement.Framework.AspNetCoreHelper.ResponseModel;
using PMS.PaidQA.Domain.Enums;
using PMS.UserManage.Application.IServices;
using IMessageService = PMS.PaidQA.Application.Services.IMessageService;
using AutoMapper;
using ProductManagement.Framework.Cache.Redis;
using System.Security.Claims;
using ProductManagement.API.Aliyun;
using ProductManagement.Framework.Foundation;
using ProductManagement.Framework.AspNetCoreHelper.Filters;

namespace PMS.SignalR.Hubs.PaidQAHub
{
    [Authorize]
    public class OrderChatHub : Hub<IOrderChatClient>
    {
        ILogger _logger;
        IOrderService _orderService;
        IMessageService _messageService;
        IUserService _userService;
        IMapper _mapper;
        protected double Latitude
        {
            get
            {
  
                if (double.TryParse(Context.GetHttpContext().Request.Cookies["latitude"], out double latitude))
                {
                    return latitude;
                }
                return 0;
            }
        }

        protected double Longitude
        {
            get
            {
                if (double.TryParse(Context.GetHttpContext().Request.Cookies["longitude"], out double latitude))
                {
                    return latitude;
                }
                return 0;
            }
        }

        IText _aliyunText;

        public OrderChatHub(
            IText aliyunText
            , IOrderService orderService
            , ILogger<OrderChatHub> logger
            , IMessageService messageService
            , IUserService userService
            , IMapper mapper)
        {
            _aliyunText = aliyunText;
            _orderService = orderService;
            _logger = logger;
            _messageService = messageService;
            _userService = userService;
            _mapper = mapper;
        }



        //[ValidateWebContent(ContentParam = "request")] 
        public async Task<ResponseResult> SendMessage(SendMessageRequest request)
        {
            Guid orderID = GetOrderID();
            Guid userId = Context.User.Identity.GetUserInfo().UserId;
            var order = _orderService.Get(orderID);
            ResponseResult result = null;
            if (!PermisionCheck(order))
            {
                result = ResponseResult.Failed("无该订单操作权限");
            }
            if (!_orderService.CheckOrderCanOperate(order))
            {
                result = ResponseResult.Failed("订单已禁止操作。");
            }
            Message message = new Message()
            {
                Content = request.Content,
                OrderID = orderID,
                MediaType = request.MediaType,
            };
            if (userId == order.CreatorID)
            {
                message.MsgType = PMS.PaidQA.Domain.Enums.MsgType.GoAsk;
            }
            else if (userId == order.AnswerID)
            {
                message.MsgType = PMS.PaidQA.Domain.Enums.MsgType.Answer;
            }

            string checkContent = request.GetContent();
            if (string.IsNullOrEmpty(checkContent) == false)
            { 
                //检测敏感词
                if (!_aliyunText.Check(new ProductManagement.API.Aliyun.Model.GarbageCheckRequest()
                {
                    scenes = new[] { "antispam" },
                    tasks = new List<ProductManagement.API.Aliyun.Model.GarbageCheckRequest.Task>() {
                      new ProductManagement.API.Aliyun.Model.GarbageCheckRequest.Task(){
                         content= checkContent,
                         dataId = Guid.NewGuid().ToString()
                      }
                      }
                }).Result
                )
                {
                    //包含敏感词
                    return new ResponseResult()
                    {
                        Msg = ResponseCode.GarbageContent.Description(),
                        status = ResponseCode.GarbageContent,
                        Succeed = false
                    };

                }

            }
            

            var addResult = await _messageService.SendMessage(message, Latitude, Longitude, SendMessageOrigin.User);
            order = _orderService.Get(order.ID);//更新订单信息
            if (addResult)
            {
                var userInfo = _userService.GetUserInfo(message.SenderID.GetValueOrDefault());
                 ReciveSendMessageResult reciveSendMessageResult  = new ReciveSendMessageResult()
                {
                    Message = _mapper.Map<MessageResult>(message),
                };
                reciveSendMessageResult.Message.SenderInfo = _mapper.Map<UserInfoResult>(userInfo);

                if (await _messageService.IsOnline(order.ID, reciveSendMessageResult.Message.ReceiveID.Value))
                {
                    //如果在线，推信息
                    var messageResults =  MessageAddSenderInfo(new List<MessageResult>() { reciveSendMessageResult.Message }, order, message.ReceiveID.GetValueOrDefault());
                    await Clients.Group(OrderChatHubHelper.OrderUserGroupName(order.ID, message.ReceiveID.Value)).ReceiveMessage(new ReceiveMessageResult()
                    {
                        Messages = messageResults
                    });
                    //更新消息为已读
                    await _messageService.UpdateMessageReadTime(message.ID);
                }
                result = ResponseResult.Success(reciveSendMessageResult, "");
            }
            else
            {
                result = ResponseResult.Failed("消息发送失败");
            }
            return result;

        }


        public async Task ReportOnline()
        {
            Guid orderID = GetOrderID();
            Guid userId = Context.User.Identity.GetUserInfo().UserId;
            var result = await _messageService.Online(orderID, userId);
        }


        /// <summary>
        /// 赋值消息的SenderInfo字段
        /// </summary>
        /// <param name="messages"></param>
        /// <param name="order"></param>
        List<MessageResult> MessageAddSenderInfo(List<MessageResult> messages, Order order, Guid reciverId)
        {
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(messages);
            List<MessageResult> newMessages = Newtonsoft.Json.JsonConvert.DeserializeObject<List<MessageResult>>(json);
            newMessages.ForEach(msg =>
            {
                msg.IsBelongSelf = msg.SenderID == reciverId;
                var userInfo = _userService.GetUserInfo(msg.SenderID.Value);
                msg.SenderInfo = _mapper.Map<UserInfoResult>(userInfo);
                if (reciverId == order.AnswerID
                   && order.IsAnonymous == true
                   && msg.SenderID == order.CreatorID)
                {
                    //只有当前登录者是专家身份，并且消息是用户发的才需要屏蔽，专家消息不处理
                    msg.SenderInfo.NickName = order.AnonyName;
                }

            });
            return newMessages;
        }



        /// <summary>
        /// 订单的一些业务权限校验
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        bool PermisionCheck(Order order)
        {
            var UserIdOrDefault = Context.User.Identity.GetUserInfo().UserId;
            if (UserIdOrDefault != order.CreatorID && UserIdOrDefault != order.AnswerID)
            {
                return false;
            }
            else
            {
                return true;
            }
        }







        public override async Task OnConnectedAsync()
        {

            HttpContext httpContext = Context.GetHttpContext();
            Order order = this._orderService.Get(GetOrderID());
            if (order == null) Context.Abort();//订单为空直接关闭连接
            Guid userId = Guid.Parse(Context.UserIdentifier);
            if (userId != order.CreatorID && userId != order.AnswerID) Context.Abort(); //没有权限连接
            await _messageService.Online(order.ID, userId); //记录上线状态
            await Groups.AddToGroupAsync(Context.ConnectionId, OrderChatHubHelper.OrderUserGroupName(order.ID,userId));
            _logger.LogDebug($"用户{userId} 加入订单通信组:{OrderChatHubHelper.OrderUserGroupName(order.ID, userId)}");
            await base.OnConnectedAsync();
        }
        public override async Task OnDisconnectedAsync(Exception exception)
        {
   
            Guid userId = Context.User.Identity.GetUserInfo().UserId;
            Guid orderID = GetOrderID();
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, OrderChatHubHelper.OrderUserGroupName(orderID, userId));
            await _messageService.Offline(orderID, userId);
            _logger.LogDebug($"用户{userId} 退出订单通信组:{OrderChatHubHelper.OrderUserGroupName(orderID, userId)}");
            await base.OnDisconnectedAsync(exception);
        }

        private Guid GetOrderID()
        {
            var httpContext = Context.GetHttpContext();
            string orderID = httpContext.Request.Query["orderId"];
            return Guid.Parse(orderID);
        }



    }
}
