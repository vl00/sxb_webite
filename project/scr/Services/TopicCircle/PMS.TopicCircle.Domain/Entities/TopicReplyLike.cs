using System;
using Dapper.Contrib.Extensions;

namespace PMS.TopicCircle.Domain.Entities
{
	[Serializable]
	[Table("TopicReplyLike")]
	public partial class TopicReplyLike
	{
		 /// <summary> 
		 /// </summary> 
		[ExplicitKey]
		public Guid Id {get;set;}

		 /// <summary> 
		 /// </summary> 
		public Guid UserId {get;set;}

		 /// <summary> 
		 /// </summary> 
		public Guid TopicReplyId {get;set;}

		 /// <summary> 
		 /// 是否点赞  0 无  1 点赞 
		 /// </summary> 
		public int Status {get;set;}

		 /// <summary> 
		 /// </summary> 
		public DateTime UpdateTime {get;set;}


	}
}