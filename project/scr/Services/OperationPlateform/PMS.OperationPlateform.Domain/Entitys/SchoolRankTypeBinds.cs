using System;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;

namespace PMS.OperationPlateform.Domain.Entitys
{
	[Serializable]
	[Table("SchoolRankTypeBinds")]
	public partial class SchoolRankTypeBinds
	{
		[Key]  
		public int Id {get;set;}

		public Guid? SchoolRankId {get;set;}

		public int? SchoolTypeId {get;set;}

		public DateTime? CreateTime {get;set;}

		public string Creator {get;set;}


	}
}