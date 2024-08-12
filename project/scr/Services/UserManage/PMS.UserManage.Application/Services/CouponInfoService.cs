using PMS.UserManage.Application.IServices;
using PMS.UserManage.Domain.Entities;
using PMS.UserManage.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PMS.UserManage.Application.Services
{
    public class CouponInfoService: ICouponInfoService
    {
        ICouponInfoRepository _couponInfoRepository;
        public CouponInfoService(ICouponInfoRepository couponInfoRepository)
        {
            _couponInfoRepository = couponInfoRepository;
        }

        public async Task<bool> CreateCoupon(CouponInfo couponInfo)
        {
            return await _couponInfoRepository.Add(couponInfo);
        }

        public async Task<CouponInfo> GetCouponInfo(Guid couponId)
        {
           return await _couponInfoRepository.Get(couponId);
        }

        public async Task<IEnumerable<CouponRule>> GetCouponRule(Guid couponId, bool direct)
        {
            return await _couponInfoRepository.GetCouponRule(couponId, direct);
        }
    }
}
