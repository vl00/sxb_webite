using System;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;

namespace PMS.OperationPlateform.Domain.Entitys
{
	[Serializable]
	[Table("School_Activity_Bind")]
	public partial class School_Activity_Bind
	{
		[Key]  
		public int Id {get;set;}

		public Guid? ActivityId {get;set;}

		public Guid? SchoolId {get;set;}

		public DateTime? CreateTime {get;set;}

		public string Creator {get;set;}


	}
}