using PMS.PaidQA.Domain.Entities;
using System;
using System.Data;
using System.Threading.Tasks;
using System.Collections.Generic;
using PMS.PaidQA.Domain.Enums;

namespace PMS.PaidQA.Repository
{
    public interface IOrderRepository : IRepository<Order>
    {

        Task<bool> IsHasProcessingOrder(Guid answerId);

        /// <summary>
        /// 流转订单原子操作
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        Task<(bool Successed, Order OriginOrder, Order NewOrder)> TransitingOrder(Order newOrder, Guid originOrderID, Guid talentUserID, Guid operatorID, string remark);

        /// <summary>
        /// 超时关闭订单原子操作
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        Task<bool> TimOutCloseOrder(Order order, Guid operatorID, string remark);

        /// <summary>
        /// 正常关闭订单原子操作
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        Task<bool> OverOrder(Order order, Guid operatorID, string remark);


        /// <summary>
        /// 微信支付回调修改订单状态
        /// </summary>
        /// <param name="orderid"></param>
        /// <param name="updateStatus"></param>
        /// <param name="userId"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        Task<bool> WechatPayCallUpdateOrder(Guid orderid, OrderStatus updateStatus, Guid userId, string remark, int paystatus);

        /// <summary>
        /// 该方法提供统一的订单状态变更记录
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="operateUserID"></param>
        /// <param name="transaction"></param>
        /// <param name="fields"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        Task<bool> UpdateAsync(Order entity
                , Guid operateUserID
                , IDbTransaction transaction = null
                , string[] fields = null
                , string remark = null, int paystatus = 0);
        /// <summary>
        /// 检查订单状态扭转的合理性
        /// </summary>
        /// <param name="orderid"></param>
        /// <param name="status">要修改的状态</param>
        /// <returns></returns>
        bool UpdateCheckLegal(Guid orderid, int status);
        Task<int> Count(string str_Where, object param);
        /// <summary>
        /// 获取达人6小时回复率
        /// </summary>
        Task<double> GetSixHoursReplyPercentByTalentUserID(Guid talentUserID);
        /// <summary>
        /// 支付异常记录订单回调情况
        /// </summary>
        /// <param name="orderid"></param>
        /// <param name="paystatus"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        Task<bool> WechatPayCallOrderLog(Guid orderid, int paystatus, string remark);

        Task<IEnumerable<Guid>> GetUnReadOrderIDs(Guid userID, OrderStatus? status = null);


        /// <summary>
        /// 获取测试环境所需要的OpenID
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<string> GetPayOpenID(Guid userId);

        /// <summary>
        /// 获取哪些OrderID是提问
        /// </summary>
        /// <param name="orderIDs"></param>
        /// <param name="talentUserID"></param>
        /// <returns></returns>
        Task<IEnumerable<Guid>> GetIsAskOrderIDs(IEnumerable<Guid> orderIDs, Guid talentUserID);

        /// <summary>
        /// 获取由待提问到超时的订单ID
        /// </summary>
        /// <param name="orderIDs">订单ID</param>
        /// <returns></returns>
        Task<IEnumerable<Guid>> GetWaitAskToTimeOutOrderIDs(IEnumerable<Guid> orderIDs);

        /// <summary>
        /// 获取转单后订单的原有真正支付的订单
        /// </summary>
        /// <param name="transitingOrderID"></param>
        /// <returns></returns>
        Task<Order> GetPayOrderBy(Guid transitingOrderID);

        /// <summary>
        /// 获取订单标签
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<OrderTag>> GetTagsByID(Guid id);

        Task<bool> ExistsEffectiveOrder(Guid talentUserId, Guid userId);


        Task<bool> ExistsOrderAsync(Guid userId);


    }
}
