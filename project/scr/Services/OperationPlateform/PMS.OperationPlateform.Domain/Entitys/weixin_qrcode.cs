using System;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;

namespace PMS.OperationPlateform.Domain.Entitys
{
	[Serializable]
	[Table("weixin_qrcode")]
	public partial class weixin_qrcode
	{
		[ExplicitKey]
		public int id {get;set;}

		public string description {get;set;}

		public string ticket {get;set;}

		public DateTime? time {get;set;}

		public Guid? template {get;set;}

		public int? scanCount {get;set;}

		public int? subscribeCount {get;set;}

		public int? unsubscribeCount {get;set;}


	}
}