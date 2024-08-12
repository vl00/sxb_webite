using System;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;

namespace PMS.OperationPlateform.Domain.Entitys
{
	[Serializable]
	[Table("sms_template")]
	public partial class sms_template
	{
		[ExplicitKey]
		public int id {get;set;}

		public string title {get;set;}

		public string text {get;set;}

		public byte type {get;set;}


	}
}