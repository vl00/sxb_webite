using System;
using Dapper.Contrib.Extensions;

namespace PMS.TopicCircle.Domain.Entities
{
	[Serializable]
	[Table("TopicView")]
	public partial class TopicView
	{
		 /// <summary> 
		 /// </summary> 
		public Guid Id {get;set;}

		 /// <summary> 
		 /// </summary> 
		public Guid CircleId {get;set;}

		 /// <summary> 
		 /// </summary> 
		public string Content {get;set;}

		 /// <summary> 
		 /// </summary> 
		public int Status {get;set;}

		 /// <summary> 
		 /// </summary> 
		public Guid? OpenUserId {get;set;}

		 /// <summary> 
		 /// </summary> 
		public int Type {get;set;}

		 /// <summary> 
		 /// </summary> 
		public byte IsQA {get;set;}

		 /// <summary> 
		 /// </summary> 
		public int TopType {get;set;}

		 /// <summary> 
		 /// </summary> 
		public DateTime? TopTime {get;set;}

		 /// <summary> 
		 /// </summary> 
		public byte IsGood {get;set;}

		 /// <summary> 
		 /// </summary> 
		public bool IsHandPick {get;set;}

		 /// <summary> 
		 /// </summary> 
		public DateTime? GoodTime {get;set;}

		 /// <summary> 
		 /// </summary> 
		public long FollowCount {get;set;}

		 /// <summary> 
		 /// </summary> 
		public long LikeCount {get;set;}

		 /// <summary> 
		 /// </summary> 
		public long ReplyCount {get;set;}

		 /// <summary> 
		 /// </summary> 
		public byte IsDeleted {get;set;}

		 /// <summary> 
		 /// </summary> 
		public Guid Creator {get;set;}

		 /// <summary> 
		 /// </summary> 
		public Guid Updator {get;set;}

		 /// <summary> 
		 /// </summary> 
		public DateTime CreateTime {get;set;}

		 /// <summary> 
		 /// </summary> 
		public DateTime UpdateTime {get;set;}

		 /// <summary> 
		 /// </summary> 
		public DateTime LastReplyTime {get;set;}

		 /// <summary> 
		 /// </summary> 
		public bool IsAutoSync {get;set;}

		 /// <summary> 
		 /// </summary> 
		public DateTime LastEditTime {get;set;}

		 /// <summary> 
		 /// </summary> 
		public DateTime? DynamicTime {get;set;}

		 /// <summary> 
		 /// </summary> 
		public DateTime TIME {get;set;}


	}
}