using System;
using Dapper.Contrib.Extensions;

namespace PMS.PaidQA.Domain.Entities
{
	[Serializable]
	[Table("CouponSpecialTalent")]
	public partial class CouponSpecialTalent
	{
		 /// <summary> 
		 /// </summary> 
		[ExplicitKey]
		public Guid Id {get;set;}

		 /// <summary> 
		 /// </summary> 
		public Guid? TalentUserId {get;set;}

		 /// <summary> 
		 /// 活动编号 
		 /// </summary> 
		public Guid ActivityId { get;set;}


	}
}