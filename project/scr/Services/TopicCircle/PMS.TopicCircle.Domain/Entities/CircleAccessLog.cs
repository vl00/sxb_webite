using System;
using Dapper.Contrib.Extensions;

namespace PMS.TopicCircle.Domain.Entities
{
	[Serializable]
	[Table("CircleAccessLog")]
	public partial class CircleAccessLog
	{
		 /// <summary> 
		 /// </summary> 
		public Guid CircleId {get;set;}

		 /// <summary> 
		 /// </summary> 
		public Guid UserId {get;set;}

		 /// <summary> 
		 /// </summary> 
		public DateTime CreateTime {get;set;}


	}
}