using PMS.UserManage.Domain.Dtos;
using PMS.UserManage.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PMS.UserManage.Application.IServices
{
    public interface ICouponTakeService
    {

        /// <summary>
        /// 判断用户是否可以领取优惠券
        /// </summary>
        /// <param name="couponId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<(bool ok, string message)> CanTake(Guid couponId, Guid userId);
        /// <summary>
        /// 判断用户是否可以领取优惠券
        /// </summary>
        /// <param name="couponId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<(bool ok, string message)> CanTake(CouponInfo couponInfo, Guid userId);

        Task<CouponTakeDto> GetLatestUnUseCoupon(Guid userId, Guid couponId);

        /// <summary>
        /// 是否达到最大领取值
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="couponId"></param>
        /// <returns></returns>
        Task<bool> HasMaxRecive(Guid userId, CouponInfo couponInfo);

        /// <summary>
        /// 是否有未使用的优惠券
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="couponId">指定的优惠券</param>
        /// <returns></returns>
        Task<bool> HasUnUseCoupon(Guid userId, Guid couponId);

        /// <summary>
        /// 判断订单是否使用了优惠券
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        Task<bool> OrderHasUseCoupon(Guid orderId);

        /// <summary>
        /// 清除订单对优惠券的预占用
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        Task<bool> ClearPreUseRecord(Guid orderId);
        /// <summary>
        /// 查询订单对优惠券的预占用
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        Task<IEnumerable<CouponOrderPreUseRecord>> GetPreUseRecordBy(Guid orderId);
        /// <summary>
        /// 插入一张订单预占用优惠券的记录
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="couponTakeId"></param>
        /// <returns></returns>
        Task<bool> InsertOrderPreUseRecord(Guid orderId, Guid couponTakeId);
        /// <summary>
        /// 领取优惠券
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="couponID"></param>
        /// <param name="originType"></param>
        /// <returns></returns>
        Task<(CouponTakeDto couponTake, string message)> ReciveCoupon(Guid userID, Guid couponID, string originType);

        /// <summary>
        /// 获取我所有的优惠券
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        Task<IEnumerable<CouponTakeDto>> GetMyCoupons(Guid userID);



        /// <summary>
        /// 销毁优惠券
        /// </summary>
        /// <param name="couponTakeId"></param>
        /// <param name="userId"></param>
        /// <param name="orderId"></param>
        /// <param name="originAmmount"></param>
        /// <param name="rulekvs"></param>
        /// <returns></returns>
        Task<bool> CancelCouponTake(Guid couponTakeId
            , Guid userId
            , Guid orderId
           );

        /// <summary>
        /// 优惠券核对
        /// </summary>
        /// <param name="couponTakeId"></param>
        /// <param name="userId"></param>
        /// <param name="rules"></param>
        /// <returns></returns>
        Task<bool> CouponVerification(Guid couponTakeId, Guid userId, List<CouponRule> rules);

        /// <summary>
        /// 计算优惠券的优惠金额
        /// </summary>
        /// <param name="coupontakeID"></param>
        /// <param name="originAmmount"></param>
        /// <returns></returns>
        Task<decimal> ComputeCouponAmmount(Guid coupontakeID, decimal originAmmount,decimal minPayAmount = 0);


        /// <summary>
        /// 计算优惠券的优惠金额
        /// </summary>
        /// <param name="couponInfo"></param>
        /// <param name="originAmmount"></param>
        /// <returns></returns>
        decimal ComputeCouponAmmount(CouponInfo couponInfo, decimal originAmmount, decimal minPayAmount = 0);

        /// <summary>
        /// 获得最佳券组合
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="originAmmount"></param>
        /// <param name="rulekvs"></param>
        /// <returns></returns>
        Task<(IEnumerable<CouponTakeDto> BestCoupons, decimal CouponAmmount)> GetBestCoupons(Guid userId
            , decimal originAmmount
            , decimal minPayAmount
            , List<CouponRule> rules);

        /// <summary>
        /// 获取某平台下用户的待使用优惠券
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="plateform"></param>
        /// <param name="rulekvs"></param>
        /// <returns></returns>
        Task<(IEnumerable<CouponTakeDto> CanUse, IEnumerable<CouponTakeDto> CantUse)> GetWaitUseCoupons(Guid userId, decimal originAmount, List<CouponRule> rules);


        /// <summary>
        /// 更新查阅时间
        /// </summary>
        /// <param name="couponTakeIds"></param>
        /// <param name="readTime"></param>
        /// <returns></returns>
        Task<bool> UpdateReadTime(IEnumerable<Guid> couponTakeIds, DateTime readTime);

        /// <summary>
        /// 退回优惠券
        /// </summary>
        /// <param name="coupontakeId"></param>
        /// <returns></returns>
        Task<bool> BackCoupon(Guid orderId);

        /// <summary>
        /// 是否已领取过某张券
        /// </summary>
        /// <param name="couponId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<bool> HasReciveCoupon(Guid couponId, Guid userId);


        /// <summary>
        /// 获取领取的优惠券
        /// </summary>
        /// <param name="takeId"></param>
        /// <returns></returns>
        Task<CouponTakeDto> GetCouponTake(Guid takeId);

        /// <summary>
        /// 获取将会过期的优惠券
        /// </summary>
        /// <param name="specialCoupons"></param>
        /// <param name="beforeHour"></param>
        /// <returns></returns>
        Task<IEnumerable<CouponTake>> GetWillExpireCoupons(List<Guid> specialCoupons = null, int beforeHour = 24);


    }
}
