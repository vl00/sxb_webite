using System;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;

namespace PMS.OperationPlateform.Domain.Entitys
{
	[Serializable]
	[Table("AppPushInfo")]
	public partial class AppPushInfo
	{
		public Guid? TaskId {get;set;}

		public string Info {get;set;}

		public DateTime CreateTime {get;set;}

		public int Sort {get;set;}


	}
}