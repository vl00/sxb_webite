using PMS.UserManage.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PMS.UserManage.Application.IServices
{
    public interface ICouponInfoService
    {
        Task<CouponInfo> GetCouponInfo(Guid couponId);


        Task<bool> CreateCoupon(CouponInfo couponInfo);



        /// <summary>
        /// 获取优惠券的规则
        /// </summary>
        /// <param name="couponId"></param>
        /// <param name="direct">0->使用规则 1->领取规则</param>
        /// <returns></returns>
        Task<IEnumerable<CouponRule>> GetCouponRule(Guid couponId, bool direct);

    }
}
