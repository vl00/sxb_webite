using System;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;

namespace PMS.OperationPlateform.Domain.Entitys
{
	[Serializable]
	[Table("weixin_reply_template")]
	public partial class weixin_reply_template
	{
		[ExplicitKey]
		public Guid id {get;set;}

		public byte type {get;set;}

		public string text {get;set;}

		public string title {get;set;}

		public string description {get;set;}

		public string url {get;set;}

		public DateTime? time {get;set;}


	}
}