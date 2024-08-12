using System;
using Dapper.Contrib.Extensions;

namespace PMS.TopicCircle.Domain.Entities
{
	[Serializable]
	[Table("Circle")]
	public partial class Circle
	{
		 /// <summary> 
		 /// 主键 
		 /// </summary> 
		[ExplicitKey]
		public Guid Id {get;set;}

		 /// <summary> 
		 /// </summary> 
		public string Name {get;set;}

		 /// <summary> 
		 /// </summary> 
		public Guid? UserId {get;set;}

		 /// <summary> 
		 /// 是否未启用 
		 /// </summary> 
		public bool IsDisable {get;set;}

		 /// <summary> 
		 /// </summary> 
		public string Intro {get;set;}

		 /// <summary> 
		 /// </summary> 
		public DateTime? ModifyTime {get;set;}

		 /// <summary> 
		 /// </summary> 
		public DateTime? CreateTime {get;set;}

		 /// <summary> 
		 /// 帖子数 
		 /// </summary> 
		public long TopicCount {get;set;}

		 /// <summary> 
		 /// 回复数 
		 /// </summary> 
		public long ReplyCount {get;set;}

		 /// <summary> 
		 /// 粉丝数 
		 /// </summary> 
		public long FollowCount {get;set;}

		 /// <summary> 
		 /// </summary> 
		public string BGColor {get;set;}


	}
}