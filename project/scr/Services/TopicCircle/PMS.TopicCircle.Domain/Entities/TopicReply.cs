using System;
using Dapper.Contrib.Extensions;

namespace PMS.TopicCircle.Domain.Entities
{
	[Serializable]
	[Table("TopicReply")]
	public partial class TopicReply
	{
		 /// <summary> 
		 /// </summary> 
		[ExplicitKey]
		public Guid Id {get;set;}

		 /// <summary> 
		 /// 深度 =0即topic 
		 /// </summary> 
		public int Depth {get;set;}

		 /// <summary> 
		 /// 回复内容 
		 /// </summary> 
		public string Content {get;set;}

		 /// <summary> 
		 /// 点赞数量 
		 /// </summary> 
		public long LikeCount {get;set;}

		 /// <summary> 
		 /// 话题Id 
		 /// </summary> 
		public Guid TopicId {get;set;}

		 /// <summary> 
		 /// 父回复Id 
		 /// </summary> 
		public Guid? ParentId {get;set;}

		 /// <summary> 
		 /// 父回复的创建人Id 
		 /// </summary> 
		public Guid? ParentUserId {get;set;}

		 /// <summary> 
		 /// 是否删除 
		 /// </summary> 
		public byte IsDeleted {get;set;}

		 /// <summary> 
		 /// 创建人 
		 /// </summary> 
		public Guid Creator {get;set;}

		 /// <summary> 
		 /// 更新人 
		 /// </summary> 
		public Guid Updator {get;set;}

		 /// <summary> 
		 /// </summary> 
		public DateTime CreateTime {get;set;}

		 /// <summary> 
		 /// </summary> 
		public DateTime UpdateTime {get;set;}

		 /// <summary> 
		 /// 第一层评论人 
		 /// </summary> 
		public Guid? FirstParentId {get;set;}


	}
}