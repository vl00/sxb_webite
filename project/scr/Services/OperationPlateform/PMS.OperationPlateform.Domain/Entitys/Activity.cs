using System;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;

namespace PMS.OperationPlateform.Domain.Entitys
{
	[Serializable]
	[Table("Activity")]
	public partial class Activity
	{
		[ExplicitKey]
		public Guid Id {get;set;}

		public string Name {get;set;}

		public DateTime? StartTime {get;set;}

		public DateTime? EndTime {get;set;}

		public int? Capacity {get;set;}

		public string Hint {get;set;}

		public DateTime? CreateTime {get;set;}

		public string Creator {get;set;}

		public DateTime? UpdateTime {get;set;}

		public string Updator {get;set;}


	}
}