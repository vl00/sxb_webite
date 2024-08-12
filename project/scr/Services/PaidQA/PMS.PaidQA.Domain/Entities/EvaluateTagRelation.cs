using System;
using Dapper.Contrib.Extensions;

namespace PMS.PaidQA.Domain.Entities
{
	[Serializable]
	[Table("EvaluateTagRelation")]
	public partial class EvaluateTagRelation
	{
		 /// <summary> 
		 /// </summary> 
		[ExplicitKey]
		public Guid ID {get;set;}

		 /// <summary> 
		 /// </summary> 
		public Guid? EvaluateID {get;set;}

		 /// <summary> 
		 /// </summary> 
		public Guid? TagID {get;set;}


	}
}