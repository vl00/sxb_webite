using System;
using Dapper.Contrib.Extensions;

namespace PMS.TopicCircle.Domain.Entities
{
	[Serializable]
	[Table("CircleCover")]
	public partial class CircleCover
	{
		 /// <summary> 
		 /// </summary> 
		[ExplicitKey]
		public Guid Id {get;set;}

		 /// <summary> 
		 /// </summary> 
		public string Url {get;set;}

		 /// <summary> 
		 /// </summary> 
		public Guid? CircleId {get;set;}

		 /// <summary> 
		 /// </summary> 
		public DateTime? Modifytime {get;set;}


	}
}