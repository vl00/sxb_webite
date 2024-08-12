using System;
using Dapper.Contrib.Extensions;

namespace PMS.TopicCircle.Domain.Entities
{
	[Serializable]
	[Table("TopicReplyAttachment")]
	public partial class TopicReplyAttachment
	{
		 /// <summary> 
		 /// </summary> 
		[ExplicitKey]
		public Guid Id {get;set;}

		 /// <summary> 
		 /// 话题Id 
		 /// </summary> 
		public Guid TopicId {get;set;}

		 /// <summary> 
		 /// 话题回复Id 
		 /// </summary> 
		public Guid TopicReplyId {get;set;}

		 /// <summary> 
		 /// 关联内容 
		 /// </summary> 
		public string Content {get;set;}

		 /// <summary> 
		 /// 关联Id 
		 /// </summary> 
		public Guid? AttachId {get;set;}

		 /// <summary> 
		 /// 外链Url 
		 /// </summary> 
		public string AttachUrl {get;set;}

		 /// <summary> 
		 /// </summary> 
		public byte IsDeleted {get;set;}

		 /// <summary> 
		 /// </summary> 
		public DateTime UpdateTime {get;set;}

		 /// <summary> 
		 /// 同TopicType 
		 /// </summary> 
		public int Type {get;set;}


	}
}