using PMS.UserManage.Domain.Dtos;
using PMS.UserManage.Domain.Entities;
using Sxb.Web.Areas.Coupon.Models.Coupon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Web.Areas.PaidQA.Models.Order
{
    public class GetCouponsResult
    {
        /// <summary>
        /// 最优惠的券组合
        /// </summary>
        public List<CouponTakeResult> BestCoupons { get; set; }

        /// <summary>
        /// 最优惠的优惠金额
        /// </summary>
        public decimal BestCouponAmmount { get; set; }
        public List<CouponTakeResult> CanUseCoupons { get; set; }
        public List<CouponTakeResult> CantUseCoupons { get; set; }
    }
}
