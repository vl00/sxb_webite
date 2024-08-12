using System;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;

namespace PMS.OperationPlateform.Domain.Entitys
{
	[Serializable]
	[Table("Weixin_auto_reply")]
	public partial class Weixin_auto_reply
	{
		[ExplicitKey]
		public Guid id {get;set;}

		public string keyword {get;set;}

		public Guid? template_id {get;set;}

		public string cityDYH {get;set;}

		public DateTime? updateTime {get;set;}


	}
}