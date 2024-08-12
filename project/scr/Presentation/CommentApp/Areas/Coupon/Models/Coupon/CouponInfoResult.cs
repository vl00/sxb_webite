using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Sxb.Web.Areas.Coupon.Models.Coupon
{
    public class CouponInfoResult
    {

		public Guid Id { get; set; }

		/// <summary> 
		/// 名字 
		/// </summary> 
		public string Name { get; set; }

		/// <summary> 
		/// 描述 
		/// </summary> 
		public string Desc { get; set; }

		/// <summary> 
		/// 1、按日期限定 2、按领券后期限 
		/// </summary> 
		public int? VaildDateType { get; set; }

		/// <summary> 
		/// 使用有效开始时间 
		/// </summary> 
		[JsonConverter(typeof(Utils.HmDateTimeConverter))]
		public DateTime? VaildStartDate { get; set; }

		/// <summary> 
		/// 使用有效结束时间 
		/// </summary> 
		[JsonConverter(typeof(Utils.HmDateTimeConverter))]
		public DateTime? VaildEndDate { get; set; }

		/// <summary> 
		/// 领券后有效期限，单位:小时 
		/// </summary> 
		public long? VaildTime { get; set; }

		/// <summary> 
		/// 每人限领 
		/// </summary> 
		public int? MaxTake { get; set; }

		/// <summary> 
		/// 库存 
		/// </summary> 
		public int Stock { get; set; }

		/// <summary> 
		/// 总量 
		/// </summary> 
		public int Total { get; set; }

		/// <summary> 
		/// 优惠券类型：1、体验券 2、折扣券 3、满减券 
		/// </summary> 
		public int? CouponType { get; set; }

		/// <summary> 
		/// 减多少 
		/// </summary> 
		public decimal Fee { get; set; }

		/// <summary> 
		/// 满多少 
		/// </summary> 
		public decimal FeeOver { get; set; }

		/// <summary> 
		/// 折扣 
		/// </summary> 
		public decimal Discount { get; set; }

		/// <summary> 
		/// 最高优惠金额 
		/// </summary> 
		public decimal MaxFee { get; set; }

		/// <summary> 
		/// 领券开始时间 
		/// </summary> 
		[JsonConverter(typeof(Utils.HmDateTimeConverter))]
		public DateTime? GetStartTime { get; set; }

		/// <summary> 
		/// 领券结束时间 
		/// </summary> 
		[JsonConverter(typeof(Utils.HmDateTimeConverter))]
		public DateTime? GetEndTime { get; set; }

		/// <summary> 
		/// 业务类型 1.上学问 
		/// </summary> 
		public int? Platform { get; set; }


		/// <summary> 
		/// 是否可叠加 
		/// </summary> 
		public bool CanMultiple { get; set; }

		/// <summary> 
		/// 跳转地址（可空） 
		/// </summary> 
		public string Link { get; set; }

		/// <summary> 
		/// </summary> 
		[JsonConverter(typeof(Utils.HmDateTimeConverter))]
		public DateTime? CreateTime { get; set; }

		/// <summary> 
		/// 创建者 
		/// </summary> 
		public Guid? Creator { get; set; }

		/// <summary>
		/// 当前用户已领取
		/// </summary>
        public bool HasRecive { get; set; }
    }
}
