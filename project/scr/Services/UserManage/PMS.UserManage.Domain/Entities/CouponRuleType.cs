using System;
using Dapper.Contrib.Extensions;

namespace PMS.UserManage.Domain.Entities
{
	[Serializable]
	[Table("CouponRuleType")]
	public partial class CouponRuleType
	{
		 /// <summary> 
		 /// </summary> 
		public int? Id {get;set;}

		 /// <summary> 
		 /// </summary> 
		public string Value {get;set;}


	}
}