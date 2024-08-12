using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Web.Areas.Coupon.Models.Coupon
{
    public class GrantCouponRequest
    {
        /// <summary>
        /// 优惠券ID
        /// </summary>
        public Guid CouponID { get; set; }


        /// <summary>
        /// 来源
        /// </summary>
        public string OriginType { get; set; }

    }
}
