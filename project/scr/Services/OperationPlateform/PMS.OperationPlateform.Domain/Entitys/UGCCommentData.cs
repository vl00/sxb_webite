using System;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;

namespace PMS.OperationPlateform.Domain.Entitys
{
	[Serializable]
	[Table("UGCCommentData")]
	public partial class UGCCommentData
	{
		[Key]  
		public int Id {get;set;}

		public Guid? ActivityId {get;set;}

		public string ActivityName {get;set;}

		public string ChannelName {get;set;}

		public int? ArticleCommentCount {get;set;}

		public int? ArticleSuperiorCommentCount {get;set;}

		public int? SchoolCommentCount {get;set;}

		public int? SchoolSuperiorCommentCount {get;set;}

		public string Creator {get;set;}

		public DateTime? CreateTime {get;set;}

		public Guid? ArticleId {get;set;}

		public Guid? SchoolId {get;set;}


	}
}