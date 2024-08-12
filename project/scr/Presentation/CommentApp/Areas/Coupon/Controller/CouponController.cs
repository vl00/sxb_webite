using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PMS.PaidQA.Application.Services;
using PMS.UserManage.Application.IServices;
using PMS.UserManage.Domain.Dtos;
using PMS.UserManage.Domain.Entities;
using PMS.UserManage.Domain.Enums;
using ProductManagement.Framework.Cache.Redis;
using Sxb.Web.Areas.Coupon.Models.Coupon;
using Sxb.Web.Controllers;
using Sxb.Web.Response;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Web.Areas.Coupon.Controller
{
    [Route("Coupon/[controller]/[action]")]
    [ApiController]
    public class CouponController : ApiBaseController
    {
        ICouponInfoService _couponInfoService;
        ICouponTakeService _couponTakeService;
        IEasyRedisClient _easyRedisClient;
        IOrderService _orderService;
        IMapper _mapper;
        public CouponController(ICouponTakeService couponTakeService, IMapper mapper, IEasyRedisClient easyRedisClient, ICouponInfoService couponInfoService, IOrderService orderService)
        {
            _couponTakeService = couponTakeService;
            _mapper = mapper;
            _easyRedisClient = easyRedisClient;
            _couponInfoService = couponInfoService;
            _orderService = orderService;
        }

        [HttpGet]
        [Authorize]
        [Description("领取优惠券")]
        public async Task<ResponseResult> ReciveCoupon([FromQuery] GrantCouponRequest request)
        {
            /*
             【说明】
             条件定义（条件限制的目标主体是用户，用条件来对用户分组。）：
                1.上学问新人（指没有使用上学帮进行过一次有效付费问答的人群）
             */
            string lockKey = $"ReciveCoupon:{UserIdOrDefault}:{request.CouponID}";
            string lockVal = $"{UserIdOrDefault}:{request.CouponID}";
            //加并发锁防止并发导致超发优惠券。
            if (await _easyRedisClient.LockTakeAsync(lockKey, lockVal, TimeSpan.FromSeconds(5))) 
            {
                var res = await _couponTakeService.ReciveCoupon(UserIdOrDefault, request.CouponID, request.OriginType);
                await _easyRedisClient.LockReleaseAsync(lockKey, lockVal);
                if (res.couponTake != null)
                {
                    return ResponseResult.Success(new
                    {
                        CouponTake = _mapper.Map<CouponTakeResult>(res.couponTake)
                    }, "领取成功");
                }
                else
                {
                    return ResponseResult.Failed(res.message);
                }
            }
            else {
                return ResponseResult.Failed("操作过快，稍后重试~~~");
            }
           
        }

        [HttpGet]
        [Authorize]
        [Description("获取我的全部券")]
        public async Task<ResponseResult<IEnumerable<CouponTakeResult>>> GetMyCoupons([FromQuery] GetMyCouponsRequest request)
        {
            var coupons = await _couponTakeService.GetMyCoupons(UserIdOrDefault);
            List<CouponTakeDto> couponTakeDtos = new List<CouponTakeDto>();
            if (request.TakeStatus.Contains(TakeStatus.WaitUse))
            {
                couponTakeDtos.AddRange(coupons.Where(s => s.Status == CouponTakeStatus.WaitUse && DateTime.Now <= s.VaildEndTime));
            }
            if (request.TakeStatus.Contains(TakeStatus.HasUse))
            {
                couponTakeDtos.AddRange(coupons.Where(s => s.Status == CouponTakeStatus.HasUse && DateTime.Now <= s.VaildEndTime));

            }
            if (request.TakeStatus.Contains(TakeStatus.Failure))
            {
                couponTakeDtos.AddRange(coupons.Where(s => DateTime.Now > s.VaildEndTime));
            }
            //更新券的阅读时间
            await _couponTakeService.UpdateReadTime(couponTakeDtos.Select(s => s.Id), DateTime.Now);
            var result = _mapper.Map<IEnumerable<CouponTakeResult>>(couponTakeDtos.OrderBy(s => s.VaildEndTime));
            return ResponseResult<IEnumerable<CouponTakeResult>>.Success(result, null);
        }





    }
}
