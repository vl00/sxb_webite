using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PMS.PaidQA.Application.Services;
using PMS.UserManage.Application.IServices;
using PMS.UserManage.Domain.Entities;
using ProductManagement.Framework.Cache.Redis;
using Sxb.Web.Areas.Coupon.Models.Coupon;
using Sxb.Web.Controllers;
using Sxb.Web.Filters;
using Sxb.Web.Response;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Web.Areas.PaidQA.Controllers
{
    [Route("PaidQA/[controller]/[action]")]
    [ApiController]
    public class CouponController : ApiBaseController
    {
        ICouponInfoService _couponInfoService;
        ICouponTakeService _couponTakeService;
        ICouponActivityService _couponActivityService;
        IMapper _mapper;
        IAccountService _accountService;
        IOrderService _orderService;
        public CouponController(ICouponInfoService couponInfoService
            , ICouponTakeService couponTakeService
            , ICouponActivityService couponActivityService
            , IMapper mapper, IAccountService accountService, IOrderService orderService)
        {
            _couponInfoService = couponInfoService;
            _couponTakeService = couponTakeService;
            _couponActivityService = couponActivityService;
            _mapper = mapper;
            _accountService = accountService;
            _orderService = orderService;
        }

        [Authorize]
        [HttpGet]
        [Description("活动信息")]
        public async Task<ResponseResult> ActivityInfos(Guid activityId)
        {
            var activity = await _couponActivityService.GetEffectActivity(activityId);
            if (activity == null)
            {
                return ResponseResult.Failed("很抱歉！本活动已结束！");
            }
            var isBindPhone = _accountService.IsBindPhone(UserIdOrDefault);
            CouponInfoResult couponInfoResult = _mapper.Map<CouponInfoResult>(await _couponInfoService.GetCouponInfo(activity.CouponId));
            var couponTake = _mapper.Map<CouponTakeResult>(await _couponTakeService.GetLatestUnUseCoupon(UserIdOrDefault, couponInfoResult.Id));
            couponInfoResult.HasRecive = couponTake != null;
            var qaexamples = await _couponActivityService.GetCouponQAExample(activity.Id);
            var broadcast = await _couponActivityService.GetBroadcastUsers();
            var randomTalent = await _couponActivityService.GetRandomSpecialTalent(activity.Id);
            return ResponseResult.Success(new
            {
                IsBindPhone = isBindPhone,
                CouponInfo = couponInfoResult,
                ExampleQAs = qaexamples,
                Broadcast = broadcast,
                RandomTalentUserId = randomTalent.TalentUserId,
                ActivityInfo = activity,
                CouponTake = couponTake
            }, "Success");

        }

        [HttpGet]
        [Description("获取滚动广播数据")]
        public async Task<ResponseResult> GetBroadcast()
        {
            var broadcast = await _couponActivityService.GetBroadcastUsers();
            return ResponseResult.Success(new
            {
                Broadcast = broadcast,
            }, "Success");

        }




        [Authorize]
        [Description("领取优惠券（通过活动的形式）")]
        [HttpGet]
        [ValidateAccoutBind]
        public async Task<ResponseResult> ReciveCoupon(string fw, Guid activityId)
        {
            var activity = await _couponActivityService.GetEffectActivity(activityId);
            if (activity == null)
            {
                return ResponseResult.Failed("很抱歉！本活动已结束！");
            }
            var res = await _couponTakeService.ReciveCoupon(UserIdOrDefault, activity.CouponId, fw);
            if (res.couponTake != null)
            {
                var randomTalent = await _couponActivityService.GetRandomSpecialTalent(activity.Id);
                return ResponseResult.Success(new
                {
                    RandomTalentUserId = randomTalent.TalentUserId,
                    CouponTake = _mapper.Map<CouponTakeResult>(res.couponTake)
                }, "领取成功");
            }
            else
            {
                return ResponseResult.Failed(res.message);
            }

        }

        [Authorize]
        [Description("获取新人优惠券")]
        [HttpGet]
        public async Task<ResponseResult> GetNewerCoupon()
        {
            //新人优惠券，已配置好。
            var couponInfo = await _couponInfoService.GetCouponInfo(Guid.Parse("9208898D-8DF0-45E5-834E-667B072F3EF9"));
            var canTakeRes = await _couponTakeService.CanTake(couponInfo, UserIdOrDefault);
            if (!canTakeRes.ok)
            {
                return ResponseResult.Success(canTakeRes.message);
            }
            else
            {
                return ResponseResult.Success(couponInfo);
            }
        }
    }
}