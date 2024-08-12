using Newtonsoft.Json;
using PMS.UserManage.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Web.Areas.Coupon.Models.Coupon
{
    public class CouponTakeResult
    {
        public Guid Id { get; set; }

        /// <summary> 
        /// 领券者ID 
        /// </summary> 
        public Guid UserId { get; set; }

        /// <summary> 
        /// 领取时间 
        /// </summary> 
        [JsonConverter(typeof(Utils.HmDateTimeConverter))]
        public DateTime GetTime { get; set; }

        /// <summary> 
        /// 有效开始时间 
        /// </summary> 
        //[JsonConverter(typeof(DateTimeConverter))]
        [JsonConverter(typeof(Utils.HmDateTimeConverter))]
        public DateTime VaildStartTime { get; set; }

        /// <summary> 
        /// 有效结束时间 
        /// </summary> 
        //[JsonConverter(typeof(DateTimeConverter))]
        [JsonConverter(typeof(Utils.HmDateTimeConverter))]
        public DateTime VaildEndTime { get; set; }

        /// <summary> 
        /// 使用时间 
        /// </summary> 
        //[JsonConverter(typeof(DateTimeConverter))]
        [JsonConverter(typeof(Utils.HmDateTimeConverter))]
        public DateTime? UsedTime { get; set; }

        /// <summary> 
        /// 1、待使用 2、已使用 3、已失效 
        /// </summary> 
        public TakeStatus Status { get; set; }

        /// <summary> 
        /// 订单ID 
        /// </summary> 
        public Guid? OrderId { get; set; }

        public string CouponNo { get; set; }

        public CouponInfoResult CouponInfo { get; set; }

        /// <summary>
        /// 是否新券
        /// </summary>
        public bool IsNewCoupon { get; set; }
    }
}
