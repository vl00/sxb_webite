using System;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;

namespace PMS.OperationPlateform.Domain.Entitys
{
	[Serializable]
	[Table("ActivityLeaveInfo")]
	public partial class ActivityLeaveInfo
	{
		[Key]  
		public int ID {get;set;}

		public string FieldName {get;set;}

		public int? FieldType {get;set;}

		public decimal Sort {get;set;}

		public Guid? ActivityId {get;set;}

		public DateTime? CreateTime {get;set;}

		public string Creator {get;set;}


	}
}