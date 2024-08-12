using System;
using Dapper.Contrib.Extensions;

namespace PMS.TopicCircle.Domain.Entities
{
	[Serializable]
	[Table("CircleFollower")]
	public partial class CircleFollower
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
		public Guid CircleId {get;set;}

		 /// <summary> 
		 /// 关注时间 
		 /// </summary> 
		public DateTime? Time {get;set;}

		 /// <summary> 
		 /// </summary> 
		public DateTime? ModifyTime {get;set;}


	}
}