using PMS.PaidQA.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace PMS.PaidQA.Repository
{
    public interface IMessageRepository : IRepository<Message>
    {

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
        /// 获取已经提问数量
        /// </summary>
        /// <param name="OrderID"></param>
        /// <returns></returns>
        Task<int> GetAskCount(Guid OrderID);

        /// <summary>
        /// 创建提问事务
        /// </summary>
        /// <param name="message"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        Task<bool> CreateQuetion(List<Message> messages, Order order);

        /// <summary>
        /// 普通用户发消息事务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message"></param>
        /// <param name="messageType"></param>
        /// <returns></returns>
        Task<(bool Success, Order Order)> UserSendMessage(Message message);


        /// <summary>
        /// 达人用户发消息事务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message"></param>
        /// <param name="messageType"></param>
        /// <returns></returns>
        Task<(bool Success, bool IsFirstReply)> TalentSendMessage(Message message);

        ///// <summary>
        ///// 发送消息
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="order"></param>
        ///// <param name="message"></param>
        ///// <param name="messageType"></param>
        ///// <param name="transaction"></param>
        ///// <returns></returns>
        //Task<bool> SendMessage<T>(Order order, Message message, T messageType, IDbTransaction transaction = null) where T : PaidQAMessage;

        /// <summary>
        /// 获取消息记录
        /// </summary>
        /// <param name="order"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        IEnumerable<Message> GetChartRecord(Order order, Guid userID);

        /// <summary>
        /// 获取订单的未读消息
        /// </summary>
        /// <param name="order"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        List<Message> GetUnReadMessage(Order order, Guid userID);

        /// <summary>
        /// 批量更新消息的阅读时间
        /// </summary>
        /// <param name="unreadlist"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        Task<bool> UpdateMessageReadTime(IEnumerable<Message> unreadlist, IDbTransaction transaction = null);
    }

}
