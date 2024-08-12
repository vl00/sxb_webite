using System;
using Dapper.Contrib.Extensions;

namespace PMS.TopicCircle.Domain.Entities
{
	[Serializable]
	[Table("Tag")]
	public partial class Tag
	{
		 /// <summary> 
		 /// </summary> 
		[Key]  
		public int Id {get;set;}

		 /// <summary> 
		 /// 标签名称 
		 /// </summary> 
		public string Name {get;set;}

		 /// <summary> 
		 /// 父级标签 
		 /// </summary> 
		public int ParentId {get;set;}

		 /// <summary> 
		 /// </summary> 
		public byte IsDeleted {get;set;}


	}
}