using System;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;

namespace PMS.OperationPlateform.Domain.Entitys
{
	[Serializable]
	[Table("AchievementRank")]
	public partial class AchievementRank
	{
		[ExplicitKey]
		public Guid Id {get;set;}

		public string Name {get;set;}

		public string Alias {get;set;}

		public int? Years {get;set;}

		public bool? IsUniversity {get;set;}

		public DateTime? CreateTime {get;set;}

		public string Creator {get;set;}

		public DateTime? UpdateTime {get;set;}

		public string Updator {get;set;}

		public byte Type {get;set;}


	}
}