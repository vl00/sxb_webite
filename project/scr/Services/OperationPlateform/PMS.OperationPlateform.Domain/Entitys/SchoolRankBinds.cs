using System;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;

namespace PMS.OperationPlateform.Domain.Entitys
{
	[Serializable]
	[Table("SchoolRankBinds")]
	public partial class SchoolRankBinds
	{
		[ExplicitKey]
		public Guid Id {get;set;}

		public Guid RankId {get;set;}

		public Guid SchoolId {get;set;}

		public decimal Sort {get;set;}

		public string Remark {get;set;}

		public DateTime? CreateTime {get;set;}

		public string Creator {get;set;}

		public DateTime? UpdateTime {get;set;}

		public string Updator {get;set;}


	}
}