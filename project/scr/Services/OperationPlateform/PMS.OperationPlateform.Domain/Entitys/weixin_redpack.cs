using System;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;

namespace PMS.OperationPlateform.Domain.Entitys
{
	[Serializable]
	[Table("weixin_redpack")]
	public partial class weixin_redpack
	{
		[ExplicitKey]
		public Guid id {get;set;}

		public int? total_amount {get;set;}

		public short min_value {get;set;}

		public short max_value {get;set;}

		public string send_name {get;set;}

		public string act_name {get;set;}

		public string wishing {get;set;}

		public string remark {get;set;}

		public DateTime? time {get;set;}


	}
}