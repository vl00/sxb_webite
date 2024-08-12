using System;
using Dapper.Contrib.Extensions;

namespace PMS.PaidQA.Domain.Entities
{
	[Serializable]
	[Table("HotRegion")]
	public partial class HotRegion
	{
		 /// <summary> 
		 /// </summary> 
		[ExplicitKey]
		public Guid ID {get;set;}

		 /// <summary> 
		 /// </summary> 
		public Guid? HotTypeID {get;set;}

		 /// <summary> 
		 /// </summary> 
		public Guid? RegionTypeID {get;set;}


	}
}