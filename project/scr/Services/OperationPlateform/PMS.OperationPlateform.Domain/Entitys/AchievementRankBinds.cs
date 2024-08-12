using System;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;

namespace PMS.OperationPlateform.Domain.Entitys
{
	[Serializable]
	[Table("AchievementRankBinds")]
	public partial class AchievementRankBinds
	{
		[Key]  
		public int Id {get;set;}

		public Guid? AchievementRankId {get;set;}

		public Guid? UniversityId {get;set;}

		public double Sort {get;set;}

		public string Creator {get;set;}

		public DateTime? CreateTime {get;set;}


	}
}