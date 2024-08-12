using System;
using Dapper.Contrib.Extensions;

namespace PMS.PaidQA.Domain.Entities
{
	[Serializable]
	[Table("EvaluateTags")]
	public partial class EvaluateTags
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
		public bool? IsValid {get;set;}


	}
}