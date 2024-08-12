using System;
using Dapper.Contrib.Extensions;

namespace PMS.TopicCircle.Domain.Entities
{
	[Serializable]
	[Table("Topping")]
	public partial class Topping
	{
		 /// <summary> 
		 /// id 
		 /// </summary> 
		[Key]  
		public long Id {get;set;}

		 /// <summary> 
		 /// 置顶内容 
		 /// </summary> 
		public string Title {get;set;}

		 /// <summary> 
		 /// 外链 
		 /// </summary> 
		public string Url {get;set;}

		 /// <summary> 
		 /// 开始置顶时间 
		 /// </summary> 
		public DateTime StartTime {get;set;}

		 /// <summary> 
		 /// 结束置顶时间Expired 
		 /// </summary> 
		public DateTime EndTime {get;set;}

		 /// <summary> 
		 /// 状态 0 禁用 1 启用 
		 /// </summary> 
		public int Status {get;set;}

		 /// <summary> 
		 /// 是否删除 
		 /// </summary> 
		public byte IsDeleted {get;set;}

		 /// <summary> 
		 /// 修改人 
		 /// </summary> 
		public Guid Updator {get;set;}

		 /// <summary> 
		 /// 修改时间 
		 /// </summary> 
		public DateTime UpdateTime {get;set;}


	}
}