using System;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;

namespace PMS.OperationPlateform.Domain.Entitys
{
	[Serializable]
	[Table("Weixin_openid")]
	public partial class Weixin_openid
	{
		[ExplicitKey]
		public string openid {get;set;}

		public Guid? userID {get;set;}

		public string gzh {get;set;}

		public DateTime? time_subscribe {get;set;}


	}
}