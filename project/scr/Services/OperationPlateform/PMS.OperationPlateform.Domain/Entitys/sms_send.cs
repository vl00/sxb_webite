using System;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;

namespace PMS.OperationPlateform.Domain.Entitys
{
	[Serializable]
	[Table("sms_send")]
	public partial class sms_send
	{
		[ExplicitKey]
		public Guid id {get;set;}

		public string mobile {get;set;}

		public string text {get;set;}

		public DateTime? time {get;set;}


	}
}