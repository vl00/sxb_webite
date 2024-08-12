using MediatR;
using PMS.MediatR.Request.PaidQA;
using PMS.UserManage.Application.IServices;
using PMS.UserManage.Domain.Dtos;
using PMS.UserManage.Domain.Entities;
using PMS.UserManage.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMS.UserManage.Application.Services
{
    public class CouponTakeService : ICouponTakeService
    {
        ICouponTakeRepository _couponTakeRepository;
        ICouponInfoRepository _couponInfoRepository;
        IMediator _mediator;

        public CouponTakeService(ICouponTakeRepository couponTakeRepository, ICouponInfoRepository couponInfoRepository, IMediator mediator)
        {
            _couponTakeRepository = couponTakeRepository;
            _couponInfoRepository = couponInfoRepository;
            _mediator = mediator;
        }

        public async Task<(bool ok,string message)> CanTake(Guid couponId,Guid userId)
        {
            CouponInfo couponInfo = await _couponInfoRepository.Get(couponId);
            return await CanTake(couponInfo, userId);
        }



        public async Task<(bool ok, string message)> CanTake(CouponInfo couponInfo, Guid userId)
        {
            //领券基本规则(前置条件) CouponInfo是少改动的
            if (couponInfo == null)
            {
                return (false, "优惠券不存在");
            }
            //1.必须领券时间段内
            if (!(DateTime.Now >= couponInfo.GetStartTime && DateTime.Now <= couponInfo.GetEndTime))
            {
                return (false, "未在优惠券有效期内。");
            }
            //上线的券才能领
            if (couponInfo.Status == 0)
            {
                return (false, "优惠券已领完。");
            }
            //余额要足才能领取
            if (couponInfo.Stock <= 0)
            {
                return (false, "优惠券已领完。");
            }

            //每人限领
            if (couponInfo.MaxTake != null)
            {
                //查出当前用户有多少张该券
                int count = await _couponTakeRepository.GetTakeCount(couponInfo.Id, userId);
                //条件判断是否可以领取。
                if (count >= couponInfo.MaxTake)
                {
                    return (false, $"每人限领{couponInfo.MaxTake}张。");
                }
            }
            var reciveRules = await _couponInfoRepository.GetCouponRule(couponInfo.Id, true);
            if (reciveRules?.Any() == true)
            {
                foreach (var rule in reciveRules)
                {
                    //ruletype 参考 iSchoolUser.dbo.CouponRuleType表
                    switch (rule.RuleType)
                    {
                        case 5:
                            if (await _mediator.Send<bool>(new OrderExistsRequest(userId)))
                            {
                                return (false, "领券失败，仅上学问新人用户领取。");
                            }
                            break;
                    }
                }
            }

            return (true, null);
        }

        public async Task<(CouponTakeDto couponTake,string message)> ReciveCoupon(Guid userID, Guid couponID, string originType)
        {
            var canTakeRes = await CanTake(couponID, userID);
            if (canTakeRes.ok)
            {
                Guid id = Guid.NewGuid();
                bool res = await _couponTakeRepository.Add(id, userID, originType, couponID);
                if (res)
                {
                    return (await _couponTakeRepository.GetAsync(id), "领取成功");
                }
                else {
                    return (await _couponTakeRepository.GetAsync(id), "领取失败。");
                }
            }
            else {
                return (null, canTakeRes.message);
            }


        }


        public async Task<IEnumerable<CouponTakeDto>> GetMyCoupons(Guid userID)
        {
            return await _couponTakeRepository.GetCoupons(userID);
        }


        public async Task<bool> CouponVerification(Guid couponTakeId,Guid userId, List<CouponRule> rules)
        {
            //核对优惠券
            CouponTakeDto couponTakeDto = await _couponTakeRepository.GetAsync(couponTakeId);
            if (couponTakeDto == null)
            {
                return false;
            }
            if (couponTakeDto.UserId != userId)
            {
                return false;
            }
            if (couponTakeDto.Status != Domain.Enums.CouponTakeStatus.WaitUse)
            {
                return false;
            }

            if (DateTime.Now < couponTakeDto.VaildStartTime || DateTime.Now > couponTakeDto.VaildEndTime)
            {
                return false;
            }

            var canUse = await _couponTakeRepository.CouponRuleJudge(couponTakeDto.CouponId, (int)couponTakeDto.CouponInfo.Platform, rules);
            return canUse;
        }

        public async Task<bool> CancelCouponTake(Guid couponTakeId
            , Guid userId
            , Guid orderId
           )
        {
            //销毁券
            bool cancel = await _couponTakeRepository.CancelCouponTake(couponTakeId, userId, orderId);
            return cancel;
        }


       public  async Task<(IEnumerable<CouponTakeDto> BestCoupons, decimal CouponAmmount)> GetBestCoupons(Guid userId
            , decimal originAmmount
            , decimal minPayAmount
            , List<CouponRule> rules)
        {
            //目前规则：没有券组合,只需要选择出优惠金额最大的那张即可。
            IEnumerable<CouponTakeDto> canUseCoupons = (await GetWaitUseCoupons(userId, originAmmount, rules)).CanUse;
            decimal maxCouponAmmount = 0;
            CouponTakeDto  couponTakeDto = null;
            foreach (var coupon in canUseCoupons)
            {
                decimal couponAmmount = ComputeCouponAmmount(coupon.CouponInfo, originAmmount, minPayAmount);
                if (couponAmmount > maxCouponAmmount)
                {
                    maxCouponAmmount = couponAmmount;
                    couponTakeDto = coupon;
                }
            }
            if (couponTakeDto != null)
            {
                return (new List<CouponTakeDto>() { couponTakeDto }, maxCouponAmmount);
            }
            else {
                return (new List<CouponTakeDto>(), 0);
            }        
        }


        public async Task<(IEnumerable<CouponTakeDto> CanUse, IEnumerable<CouponTakeDto> CantUse)> GetWaitUseCoupons(Guid userId,decimal originAmount ,List<CouponRule> rules)
        {

            //获取某业务平台下待使用的优惠券集合。
            var coupons = await _couponTakeRepository.GetWaitUseCoupons(userId);
            //获取满足业务规则的优惠券ID列表。
            var couponIds = await _couponTakeRepository.GetCouponIdsByRules(rules);
            List<CouponTakeDto> canUse = new List<CouponTakeDto>();
            List<CouponTakeDto> cantUse = new List<CouponTakeDto>();
            foreach (var coupon in coupons)
            {
                if (couponIds.Contains(coupon.CouponId)
                    &&
                    CouponTypeRulesJudge(coupon.CouponInfo,originAmount)
                    &&
                    coupon.VaildStartTime<=DateTime.Now)
                {
                    canUse.Add(coupon);
                }
                else {
                    cantUse.Add(coupon);
                }
            }
            return (canUse, cantUse);
           }


        /// <summary>
        /// 优惠券自身的使用限制规则
        /// </summary>
        /// <param name="couponInfo"></param>
        /// <param name="originAmount"></param>
        /// <returns></returns>
        bool CouponTypeRulesJudge(CouponInfo couponInfo,decimal originAmount)
        {
            //如果原价格为0，直接不能使用优惠券。
            if (originAmount <= 0) {
                return false;
            }
            switch (couponInfo.CouponType)
            {
                case Domain.Enums.CouponInfoCouponType.TiYan:
                    return  originAmount>=couponInfo.MaxFee;
                case Domain.Enums.CouponInfoCouponType.ZheKou:
                    return true;
                case Domain.Enums.CouponInfoCouponType.ManJian:
                    return originAmount >= couponInfo.FeeOver;
                default:
                    return true;
            }
        }

        /// <summary>
        /// 计算优惠券的优惠金额
        /// </summary>
        /// <param name="couponInfo">优惠券</param>
        /// <param name="originAmmount">原价</param>
        /// <returns></returns>
        public async Task<decimal> ComputeCouponAmmount(Guid coupontakeID, decimal originAmmount, decimal minPayAmount = 0)
        {
            //核销
            CouponTakeDto couponTakeDto = await _couponTakeRepository.GetAsync(coupontakeID);
            return ComputeCouponAmmount(couponTakeDto.CouponInfo, originAmmount, minPayAmount);

        }

        /// <summary>
        /// 计算优惠券的优惠金额
        /// </summary>
        /// <param name="couponInfo">优惠券</param>
        /// <param name="originAmmount">原价</param>
        /// <returns></returns>
        public decimal ComputeCouponAmmount(CouponInfo couponInfo, decimal originAmmount,decimal minPayAmount=0)
        {
            decimal couponAmmount = 0;
            switch (couponInfo.CouponType)
            {
                case Domain.Enums.CouponInfoCouponType.TiYan:
                    //体验券以指定的价格。
                    couponAmmount = (originAmmount - couponInfo.MaxFee);
                    break;
                case Domain.Enums.CouponInfoCouponType.ZheKou:
                    couponAmmount = (originAmmount - originAmmount * couponInfo.Discount);
                    break;
                case Domain.Enums.CouponInfoCouponType.ManJian:
                    if (originAmmount >= couponInfo.FeeOver)
                    {
                        couponAmmount = couponInfo.Fee;
                    }
                    break;
            }
            //抵扣金额需要满足条件：原价-最小支付价格>=抵扣金额，如果不满足，取抵扣金额的最大极限。
            if ((originAmmount - minPayAmount) < couponAmmount)
            {
                couponAmmount = (originAmmount - minPayAmount); //取最大值
            }


            return couponAmmount;

        }

        public Task<bool> UpdateReadTime(IEnumerable<Guid> couponTakeIds, DateTime readTime)
        {
            return _couponTakeRepository.UpdateReadTime(couponTakeIds, readTime);
        }

        public Task<bool> BackCoupon(Guid orderId)
        {
            return _couponTakeRepository.BackCoupon(orderId);
        }

        public Task<bool> InsertOrderPreUseRecord(Guid orderId, Guid couponTakeId)
        {
            return _couponTakeRepository.InsertOrderPreUseRecord(orderId, couponTakeId);
        }

        public Task<IEnumerable<CouponOrderPreUseRecord>> GetPreUseRecordBy(Guid orderId)
        {
            return _couponTakeRepository.GetPreUseRecordBy(orderId);
        }

        public async Task<bool> ClearPreUseRecord(Guid orderId)
        {
            return await _couponTakeRepository.ClearPreUseRecord(orderId);
        }

        public async Task<bool> OrderHasUseCoupon(Guid orderId)
        {
            return await _couponTakeRepository.OrderHasUseCoupon(orderId);
        }

        public async Task<bool> HasReciveCoupon(Guid couponId, Guid userId)
        {
            return (await _couponTakeRepository.GetTakeCount(couponId, userId)) > 0;
        }

        public async Task<bool> HasUnUseCoupon(Guid userId, Guid couponId)
        {
            return  (await _couponTakeRepository.GetUnUseCouponCount(userId,couponId) )> 0;
        }

        public async Task<bool> HasMaxRecive(Guid userId,CouponInfo couponInfo)
        {
            //每人限领
            if (couponInfo.MaxTake != null)
            {
                //查出当前用户有多少张该券
                int count = await _couponTakeRepository.GetTakeCount(couponInfo.Id, userId);
                //条件判断是否可以领取。
                if (count >= couponInfo.MaxTake)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else {
                return false;
            }
        }

        public async Task<CouponTakeDto> GetLatestUnUseCoupon(Guid userId, Guid couponId)
        {
            return await _couponTakeRepository.GetLatestUnUseCoupon(userId, couponId);
        }

        public async Task<CouponTakeDto> GetCouponTake(Guid takeId)
        {
           return await  _couponTakeRepository.GetAsync(takeId);
        }

        public async Task<IEnumerable<CouponTake>> GetWillExpireCoupons(List<Guid> specialCoupons = null, int beforeHour = 24)
        {
            return await _couponTakeRepository.GetWillExpireCoupons(specialCoupons, beforeHour);
        }
    }
}
