using System;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;

namespace PMS.OperationPlateform.Domain.Entitys
{
	[Serializable]
	[Table("Activity_Plateform_Bind")]
	public partial class Activity_Plateform_Bind
	{
		[Key]  
		public int Id {get;set;}

		public string PlateformName {get;set;}

		public Guid? ActivityId {get;set;}

		public DateTime? CreateTime {get;set;}

		public string Creator {get;set;}


	}
}