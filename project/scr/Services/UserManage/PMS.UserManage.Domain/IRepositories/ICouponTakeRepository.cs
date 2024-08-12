using PMS.UserManage.Domain.Dtos;
using PMS.UserManage.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PMS.UserManage.Domain.IRepositories
{
    public interface ICouponTakeRepository
    {

        /// <summary>
        /// 获取用户某张最新领取却未使用的指定种类券
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="couponId"></param>
        /// <returns></returns>
        Task<CouponTakeDto> GetLatestUnUseCoupon(Guid userId, Guid couponId);
        /// <summary>
        /// 获取用户领取到指定种类未使用的优惠券的数量
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="couponId"></param>
        /// <returns></returns>
        Task<int> GetUnUseCouponCount(Guid userId, Guid couponId);
        Task<bool> OrderHasUseCoupon(Guid orderId);
        Task<bool> ClearPreUseRecord(Guid orderId);
        Task<IEnumerable<CouponOrderPreUseRecord>> GetPreUseRecordBy(Guid orderId);
        Task<bool> InsertOrderPreUseRecord(Guid orderId, Guid couponTakeId);
        Task<bool> Add(Guid Id, Guid userId, string originType, Guid couponId);
        Task<int> GetTakeCount(Guid couponID, Guid userID);
        Task<IEnumerable<CouponTakeDto>> GetCoupons(Guid userID);
        Task<CouponTakeDto> GetAsync(Guid id);

        Task<bool> CouponRuleJudge(Guid couponID
         , int plateform
         , List<CouponRule> rules);

        Task<bool> CancelCouponTake(Guid couponTakeID,Guid userID,Guid orderID);

        Task<IEnumerable<CouponTakeDto>> GetWaitUseCoupons(Guid userID);


        /// <summary>
        /// 查询满足当前业务规则的优惠券Ids
        /// </summary>
        /// <param name="plateform"></param>
        /// <param name="rulekvs"></param>
        /// <returns></returns>
        Task<List<Guid>> GetCouponIdsByRules(List<CouponRule> rules);

       
        Task<bool> UpdateReadTime(IEnumerable<Guid> couponTakeIds, DateTime readTime);

        Task<bool> BackCoupon(Guid orderId);

        /// <summary>
        /// 获取将会过期的优惠券
        /// </summary>
        /// <param name="specialCoupons"></param>
        /// <param name="beforeHour"></param>
        /// <returns></returns>
        Task<IEnumerable<CouponTake>> GetWillExpireCoupons(List<Guid> specialCoupons = null, int beforeHour = 24);
    }
}
