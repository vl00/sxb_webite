using System;
using Dapper.Contrib.Extensions;

namespace PMS.PaidQA.Domain.Entities
{
	[Serializable]
	[Table("RegionType")]
	public partial class RegionType
	{
		 /// <summary> 
		 /// </summary> 
		[ExplicitKey]
		public Guid ID {get;set;}

		 /// <summary> 
		 /// </summary> 
		public string Name {get;set;}

		 /// <summary> 
		 /// </summary> 
		public Guid? PID {get;set;}

		 /// <summary> 
		 /// </summary> 
		public int? Sort {get;set;}

		 /// <summary> 
		 /// </summary> 
		public bool? IsValid {get;set;}
	}
}