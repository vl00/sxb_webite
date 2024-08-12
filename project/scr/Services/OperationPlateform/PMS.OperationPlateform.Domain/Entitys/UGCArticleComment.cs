using System;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;

namespace PMS.OperationPlateform.Domain.Entitys
{
	[Serializable]
	[Table("UGCArticleComment")]
	public partial class UGCArticleComment
	{
		[Key]  
		public int Id {get;set;}

		public Guid? ArticleId {get;set;}

		public int? CommentTypeId {get;set;}

		public string CommentContent {get;set;}

		public Guid? UserId {get;set;}

		public DateTime? CreateTime {get;set;}

		public string Creator {get;set;}


	}
}