using System;
using Dapper.Contrib.Extensions;

namespace PMS.TopicCircle.Domain.Entities
{
	[Serializable]
	[Table("TopicTag")]
	public partial class TopicTag
	{
		 /// <summary> 
		 /// </summary> 
		public Guid TopicId {get;set;}

		 /// <summary> 
		 /// </summary> 
		public int TagId {get;set;}


	}
}