using System;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;

namespace PMS.OperationPlateform.Domain.Entitys
{
	[Serializable]
	[Table("AppPushRecord")]
	public partial class AppPushRecord
	{
		public Guid? UUId {get;set;}

		public Guid? UserId {get;set;}

		public string DeviceToken {get;set;}

		public Guid? TaskId {get;set;}

		public DateTime? CreateTime {get;set;}


	}
}