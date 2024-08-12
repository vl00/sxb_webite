using System;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;

namespace PMS.OperationPlateform.Domain.Entitys
{
	[Serializable]
	[Table("article_Activity_Bind")]
	public partial class article_Activity_Bind
	{
		[Key]  
		public int Id {get;set;}

		public Guid? ActivityId {get;set;}

		public Guid? ArticleId {get;set;}

		public DateTime? CreateTime {get;set;}

		public string Creator {get;set;}


	}
}