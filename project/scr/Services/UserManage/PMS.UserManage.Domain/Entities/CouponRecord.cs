using System;
using Dapper.Contrib.Extensions;

namespace PMS.UserManage.Domain.Entities
{
	[Serializable]
	[Table("CouponRecord")]
	public partial class CouponRecord
	{
		 /// <summary> 
		 /// </summary> 
		[ExplicitKey]
		public Guid Id {get;set;}

		 /// <summary> 
		 /// </summary> 
		public Guid CouponTakeID { get;set;}

		 /// <summary> 
		 /// </summary> 
		public string Remark {get;set;}

		 /// <summary> 
		 /// </summary> 
		public DateTime? CreateTime {get;set;}

		 /// <summary> 
		 /// </summary> 
		public Guid? Creator {get;set;}


	}
}