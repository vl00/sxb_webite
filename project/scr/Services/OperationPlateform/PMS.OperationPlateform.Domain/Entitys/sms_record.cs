using System;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;

namespace PMS.OperationPlateform.Domain.Entitys
{
	[Serializable]
	[Table("sms_record")]
	public partial class sms_record
	{
		[ExplicitKey]
		public Guid id {get;set;}

		public string sid {get;set;}

		public string mobile {get;set;}

		public string text {get;set;}

		public DateTime? time {get;set;}

		public bool? type {get;set;}

		public string status {get;set;}


	}
}