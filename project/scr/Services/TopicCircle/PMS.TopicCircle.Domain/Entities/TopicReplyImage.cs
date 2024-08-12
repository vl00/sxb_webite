using System;
using Dapper.Contrib.Extensions;

namespace PMS.TopicCircle.Domain.Entities
{
	[Serializable]
	[Table("TopicReplyImage")]
	public partial class TopicReplyImage
	{
		 /// <summary> 
		 /// </summary> 
		[ExplicitKey]
		public Guid Id {get;set;}

		 /// <summary> 
		 /// 话题回复Id 
		 /// </summary> 
		public Guid TopicReplyId {get;set;}

		 /// <summary> 
		 /// 图片链接 
		 /// </summary> 
		public string Url {get;set;}

		 /// <summary> 
		 /// 图片排序 
		 /// </summary> 
		public int Sort {get;set;}

		 /// <summary> 
		 /// </summary> 
		public byte IsDeleted {get;set;}


	}
}