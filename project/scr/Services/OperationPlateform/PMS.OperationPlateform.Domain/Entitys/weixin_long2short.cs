using System;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;

namespace PMS.OperationPlateform.Domain.Entitys
{
	[Serializable]
	[Table("weixin_long2short")]
	public partial class weixin_long2short
	{
		public string long_url {get;set;}

		public string short_url {get;set;}

		public string short_url_weixinLogin {get;set;}


	}
}