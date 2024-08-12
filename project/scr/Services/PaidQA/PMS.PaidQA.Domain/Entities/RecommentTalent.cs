using System;
using Dapper.Contrib.Extensions;

namespace PMS.PaidQA.Domain.Entities
{
	[Serializable]
	[Table("RecommentTalent")]
	public partial class RecommentTalent
	{
		 /// <summary> 
		 /// </summary> 
		[ExplicitKey]
		public Guid ID {get;set;}

		 /// <summary> 
		 /// </summary> 
		public Guid UserID {get;set;}

		 /// <summary> 
		 /// 领域类型ID 
		 /// </summary> 
		public Guid RegionTypeID {get;set;}

		 /// <summary> 
		 /// </summary> 
		public int Sort {get;set;}


	}
}