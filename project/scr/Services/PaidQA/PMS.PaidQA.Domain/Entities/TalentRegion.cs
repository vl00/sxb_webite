using System;
using Dapper.Contrib.Extensions;

namespace PMS.PaidQA.Domain.Entities
{
	[Serializable]
	[Table("TalentRegion")]
	public partial class TalentRegion
	{
		 /// <summary> 
		 /// </summary> 
		[ExplicitKey]
		public Guid ID {get;set;}

		 /// <summary> 
		 /// </summary> 
		public Guid? UserID {get;set;}

		 /// <summary> 
		 /// </summary> 
		public Guid? RegionTypeID {get;set;}


	}
}