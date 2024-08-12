using System;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;

namespace PMS.OperationPlateform.Domain.Entitys
{
	[Serializable]
	[Table("UGCArticleQA")]
	public partial class UGCArticleQA
	{
		[Key]  
		public int Id {get;set;}

		public Guid? ArticleId {get;set;}

		public int? AnserTargetId {get;set;}

		public int? CommentTypeId {get;set;}

		public string QAContent {get;set;}

		public Guid? UserId {get;set;}

		public DateTime? CreateTime {get;set;}

		public string Creator {get;set;}


	}
}