using PMS.PaidQA.Domain.Entities;
using PMS.PaidQA.Domain.EntityExtend;
using PMS.PaidQA.Domain.Enums;
using ProductManagement.Infrastructure.Toolibrary;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PMS.PaidQA.Application.Services
{
    public interface IOrderService : IApplicationService<Order>
    {



        /// <summary>
        /// 获取剩余指定小时的有效订单
        /// </summary>
        /// <param name="hours"></param>
        /// <returns></returns>
        Task<IEnumerable<Order>> GetRemainHourOrders(int hours);


        /// <summary>
        /// 获取订单创建者有未读消息的订单
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<Order>> GetCreatorHasNewMsgOrders(int timeout_h = 5);

        Task<bool> IsHasProcessingOrder(Guid answerId);

        /// <summary>
        /// 判断用户对这个专家是否有一张未支付的订单
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        Task<Order> GetUnPayOrder(Guid userID, Guid talentUserID);

        /// <summary>
        /// 获取超过11小时未回答的订单
        /// </summary>
        /// <returns></returns>
        IEnumerable<Order> GetOverTimeAnswerOrders();




        /// <summary>
        /// 获取已提问次数
        /// </summary>
        /// <param name="OrderID"></param>
        /// <returns></returns>
        int GetHasAskCount(Order order);

        /// <summary>
        /// 判断是否第一次提问
        /// </summary>
        /// <param name="OrderID"></param>
        /// <returns></returns>
        bool IsFirstAsk(Order order);


        /// <summary>
        /// 判断是否最后一次提问
        /// </summary>
        /// <param name="OrderID"></param>
        /// <returns></returns>
        bool IsLastAsk(Order order);

        /// <summary>
        /// 超时关闭订单（异常关闭）
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        Task<bool> TimeOutCloseOrder(Order order, Guid operatorID, string remark);

        /// <summary>
        /// 超时结束订（正常结束）
        /// </summary>
        /// <returns></returns>
        Task<bool> TimeOutOverOrder(Order order, Guid operatorID, string remark);


        /// <summary>
        /// 支付成功回调
        /// </summary>
        /// <param name="orderid"></param>
        /// <param name="userid"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        Task<bool> PayCallBack(Guid orderid, Guid userid, int paystatus, string remark);
        /// <summary>
        /// 创建订单
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        Task<(Order Order, bool isReCreate)> CreateAsync(Order order);

        /// <summary>
        /// 转单
        /// </summary>
        /// <param name="originOrder"></param>
        /// <param name="tagetOrder"></param>
        /// <returns></returns>

        Task<Order> TransitingOrder(Order originOrder, Guid operatorID, TalentSetting targetTalent);

        Task<PageResultEx<OrderPageExtend>> Page(int pageIndex, int pageSize, Guid userID, IEnumerable<int> status, bool isAsk);

        /// <summary>
        /// 获取专家6小时回复率
        /// </summary>
        /// <param name="talentUserID"></param>
        /// <returns></returns>
        Task<double> GetTalentInSixHourReplyPercent(Guid talentUserID);


        /// <summary>
        /// 批量传入订单ID获取提问者的昵称与头像
        /// </summary>
        /// <param name="ids"></param>
        /// <returns>(orderID, CreatorID, NickName, HeadImgUrl)</returns>
        Task<IEnumerable<(Guid, Guid, string, string)>> GetQuestionerInfoByOrderIDs(IEnumerable<Guid> ids);

        Task<IEnumerable<Order>> ListByIDs(IEnumerable<Guid> ids);


        /// <summary>
        /// 获取待转单列表
        /// </summary>
        /// <returns></returns>
        IEnumerable<Order> GetWaitTransitingOrders();

        /// <summary>
        /// 获取用户与目标专家进行中的订单列表
        /// </summary>
        /// <param name="operatorID"></param>
        /// <param name="talentUserID"></param>
        /// <returns></returns>
        Task<IEnumerable<Order>> GetValidOrders(Guid operatorID, Guid talentUserID);

        /// <summary>
        /// 结束订单
        /// </summary>
        /// <param name="order"></param>
        /// <param name="operatorID"></param>
        /// <returns></returns>
        Task<bool> OverOrder(Order order, Guid operatorID, string remark);

        /// <summary>
        /// 拒绝订单
        /// </summary>
        /// <param name="order"></param>
        /// <param name="operatorID"></param>
        /// <returns></returns>
        Task<bool> RefusOrder(Order order, Guid operatorID);


        /// <summary>
        /// 拒绝转单
        /// </summary>
        /// <param name="order"></param>
        /// <param name="operatorID"></param>
        /// <returns></returns>
        Task<bool> RefusTransiting(Message message, Guid operatorID);


        /// <summary>
        /// 获取超时订单列表
        /// </summary>
        /// <returns></returns>
        IEnumerable<Order> GetOverTimeOrders();

        bool CheckOrderIsOverTime(Order order);

        /// <summary>
        /// 更新订单
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="operatorID"></param>
        /// <param name="fileds"></param>
        /// <returns></returns>
        Task<bool> UpdateAsync(Order entity, Guid operatorID, string[] fileds, string remark);


        /// <summary>
        /// 获取用户未读订单ID
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        Task<IEnumerable<Guid>> GetUnReadOrderIDs(Guid userID, OrderStatus? status = null);

        /// <summary>
        /// 获取测试所需要的OpenID, 只在测试环境中使用。
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<string> GetPayOpenID(Guid userId);

        /// <summary>
        /// 检查订单能否操作
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        bool CheckOrderCanOperate(Order order);

        /// <summary>
        /// 检查传入的OrderID哪些是提问
        /// </summary>
        /// <param name="orderIDs"></param>
        /// <param name="talentUserID"></param>
        /// <returns></returns>
        Task<IEnumerable<Guid>> CheckIsAskOrderIDs(IEnumerable<Guid> orderIDs, Guid talentUserID);


        /// <summary>
        /// 部分退款
        /// </summary>
        /// <param name="order"></param>
        /// <param name="ammount"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        Task<bool> RefundPart(Order order, decimal ammount, string reason);

        /// <summary>
        /// 订单全额退款。
        /// </summary>
        /// <param name="order"></param>
        /// <param name="resaon"></param>
        /// <returns></returns>
        Task<bool> Refund(Order order, string reason);


        /// <summary>
        /// 获取用户所有的订单。
        /// </summary>
        /// <returns></returns>
        [Obsolete("仅供调试使用")]
        Task<IEnumerable<Order>> GetBy(Guid userID);


        /// <summary>
        /// 获取转单后订单的原有真正支付的订单
        /// </summary>
        /// <param name="transitingOrderID"></param>
        /// <returns></returns>
        Task<Order> GetPayOrderBy(Guid transitingOrderID);

        /// <summary>
        /// 从缓存里读订单，缓存在与聊天界面的各个变更事件里set入 。
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Order> GetFromCache(Guid id);

        /// <summary>
        /// 将订单set入缓存
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<bool> SetToCache(Guid id);

        /// <summary>
        /// 获取订单的标签
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<OrderTag>> GetOrderTags(Guid orderID);
        Task<IEnumerable<Order>> GetByTime(DateTime startTime, DateTime endTime);



        /// <summary>
        /// 是否存在一张有效订单
        /// </summary>
        /// <param name="talentUserId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<bool> ExistsEffectiveOrder(Guid talentUserId,Guid userId);

        /// <summary>
        /// 检查用户是否存在订单记录（不包含状态为0的）
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<bool> ExistsOrderAsync(Guid userId);

        /// <summary>
        /// 获取待提问状态并且超过12小时的订单
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<Order>> GetWaitAskAndTimeOutOrders(int timeOutHour=12);

        /// <summary>
        /// 获取待回复状态并且超过12小时的订单
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<Order>> GetWaitReplyAndTimeOutOrders(int timeOutHour = 12);

    }
}
