using System;
using Dapper.Contrib.Extensions;

namespace PMS.UserManage.Domain.Entities
{
	[Serializable]
	[Table("CouponRule")]
	public partial class CouponRule
	{
		 /// <summary> 
		 /// </summary> 
		[ExplicitKey]
		public int Id {get;set;}

		 /// <summary> 
		 /// 优惠券ID 
		 /// </summary> 
		public Guid? CouponId {get;set;}

		 /// <summary> 
		 /// 业务类型 
		 /// </summary> 
		public int? Platform {get;set;}

		 /// <summary> 
		 /// 规则类型 
		 /// </summary> 
		public int? RuleType {get;set;}

		 /// <summary> 
		 /// 范围值 
		 /// </summary> 
		public string RuleValue {get;set;}


	}
}