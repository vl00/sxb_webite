using System;
using Dapper.Contrib.Extensions;
using Newtonsoft.Json;
using PMS.UserManage.Domain.Enums;

namespace PMS.UserManage.Domain.Entities
{
	[Serializable]
	[Table("CouponTake")]
	public partial class CouponTake
	{
		 /// <summary> 
		 /// </summary> 
		[ExplicitKey]
		public Guid Id {get;set;}

		 /// <summary> 
		 /// 领券者ID 
		 /// </summary> 
		public Guid? UserId {get;set;}

		 /// <summary> 
		 /// </summary> 
		public Guid CouponId {get;set;}

		/// <summary> 
		/// 领取时间 
		/// </summary>
		public DateTime? GetTime {get;set;}

		 /// <summary> 
		 /// 有效开始时间 
		 /// </summary> 
		public DateTime? VaildStartTime {get;set;}

		 /// <summary> 
		 /// 有效结束时间 
		 /// </summary> 
		public DateTime? VaildEndTime {get;set;}

		 /// <summary> 
		 /// 使用时间 
		 /// </summary> 
		public DateTime? UsedTime {get;set;}

		 /// <summary> 
		 /// 1、待使用 2、已使用 
		 /// </summary> 
		public CouponTakeStatus Status {get;set;}

		 /// <summary> 
		 /// 订单ID 
		 /// </summary> 
		public Guid? OrderId {get;set;}

		 /// <summary> 
		 /// 来源类型 
		 /// </summary> 
		public string OriginType {get;set;}


		[Write(false)]
		public int CouponNo {get;set;}

        public DateTime? ReadTime { get; set; }



    }
}