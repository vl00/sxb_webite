using PMS.School.Domain.Dtos;
using Sxb.Web.Areas.Coupon.Models.Coupon;
using System;
using System.Collections.Generic;

namespace Sxb.Web.Areas.PaidQA.Models.Talent
{
    public class GetDetailResponse
    {
        public Guid UserID { get; set; }
        public string Introduction { get; set; }
        public IEnumerable<string> RegionTypeNames { get; set; }
        public string NickName { get; set; }
        public IEnumerable<string> GradeNames { get; set; }
        public string AuthName { get; set; }
        public string LevelName { get; set; }
        public string Score { get; set; }
        public string ReplyPercent { get; set; }
        public string HeadImgUrl { get; set; }
        public bool IsFollowed { get; set; }
        public string Price { get; set; }
        public bool IsEnable { get; set; }
        public int TalentType { get; set; }

        public List<string> Covers { get; set; }

        /// <summary>
        /// 最优惠的券组合
        /// </summary>
        public List<CouponTakeResult> BestCoupons { get; set; } = new List<CouponTakeResult>();

        /// <summary>
        /// 最优惠的优惠金额
        /// </summary>
        public decimal BestCouponAmmount { get; set; }


        public List<CouponTakeResult> CanUseCoupons { get; set; } = new List<CouponTakeResult>();


        public SchoolResult School { get; set; }
    }
}
