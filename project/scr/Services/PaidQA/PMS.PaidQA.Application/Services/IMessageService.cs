using PMS.PaidQA.Domain.Entities;
using PMS.PaidQA.Domain.Enums;
using PMS.PaidQA.Domain.Message;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PMS.PaidQA.Application.Services
{
    public interface IMessageService : IApplicationService<Message>
    {
        /// <summary>
        /// 推送消息到接收者未读队列中
        /// </summary>
        /// <param name="orderID"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        Task<bool> PushUnReadMsg(Guid orderID, Guid reciverID, Message message);

        /// <summary>
        /// 创建提问
        /// </summary>
        /// <param name="order"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        Task<bool> CreateQuetion(Order order, List<Message> message);

        /// <summary>
        /// 获取已经提问次数
        /// </summary>
        /// <param name="OrderID"></param>
        /// <returns></returns>
        Task<int> GetAskCount(Guid OrderID);
        Task<IEnumerable<Message>> GetByOrders(IEnumerable<Guid> orderIDs);


        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="operateUserId"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        Task<bool> SendMessage(Message message, double latitude=0, double longitude=0,SendMessageOrigin sendMessageOrigin = SendMessageOrigin.User);


        /// <summary>
        /// 发送泛型消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="order"></param>
        /// <param name="message"></param>
        /// <param name="messageType"></param>
        /// <returns></returns>
        Task<bool> SendMessage<T>(Message message, T messageType, double latitude =0, double longitude =0, SendMessageOrigin sendMessageOrigin= SendMessageOrigin.System) where T : PaidQAMessage;

        /// <summary>
        /// 拉取聊天记录
        /// </summary>
        /// <param name="order"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        Task<IEnumerable<Message>> GetChartRecord(Order order, Guid userID, double latitude=0, double longitude=0);

        /// <summary>
        /// 轮询未读消息
        /// </summary>
        /// <param name="order"></param>
        /// <param name="userID"></param>
        /// <param name="repeatTime">轮询阈值</param>
        /// <returns></returns>
        Task<List<Message>> GetUnReadMessage(Order order, Guid userID, double latitude=0, double longitude=0, int repeatTime = 15);


        /// <summary>
        /// 获取订单的提问
        /// </summary>
        /// <param name="orderID"></param>
        /// <returns></returns>
        Task<Message> GetOrderQuetion(Guid orderID);
        /// <summary>
        /// 批量获取订单的提问
        /// </summary>
        /// <param name="orderIDs"></param>
        /// <returns></returns>
        Task<IEnumerable<Message>> GetOrdersQuetion(IEnumerable<Guid> orderIDs);


        /// <summary>
        /// 更新转单消息的转单后订单ID
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        Task UpdateTrantingMsgToHasTranstiy(Order oldOrder, Order newOrder);

        /// <summary>
        /// 判断用户在该订单上是否或者。
        /// </summary>
        /// <param name="orderID"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        Task<bool> IsLive(Guid orderID, Guid userID);

        Task<bool> Online(Guid orderId, Guid userId);

        Task<bool> Offline(Guid orderId, Guid userId);

        Task<bool> IsOnline(Guid orderId, Guid userId);

        Task<bool> UpdateMessageReadTime(Guid id);

    }
}