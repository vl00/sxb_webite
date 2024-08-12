using System;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;

namespace PMS.OperationPlateform.Domain.Entitys
{
	[Serializable]
	[Table("weixin_redpack_send_history")]
	public partial class weixin_redpack_send_history
	{
		[ExplicitKey]
		public string mch_billno {get;set;}

		public string openid {get;set;}

		public short amount {get;set;}

		public Guid? redpackID {get;set;}

		public string remark {get;set;}

		public DateTime? time {get;set;}

		public string responseResult {get;set;}


	}
}