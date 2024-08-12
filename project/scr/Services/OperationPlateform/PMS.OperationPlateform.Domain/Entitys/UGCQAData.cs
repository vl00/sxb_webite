using System;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;

namespace PMS.OperationPlateform.Domain.Entitys
{
	[Serializable]
	[Table("UGCQAData")]
	public partial class UGCQAData
	{
		[Key]  
		public int Id {get;set;}

		public Guid? ActivityId {get;set;}

		public string ActivityName {get;set;}

		public byte[] ChannelName {get;set;}

		public int? ArticleQACommentCount {get;set;}

		public int? ArticleSuperiorQACount {get;set;}

		public int? SchoolQACount {get;set;}

		public int? SchoolSuperiorQACount {get;set;}

		public string Creator {get;set;}

		public DateTime? CreateTime {get;set;}

		public Guid? ArticleId {get;set;}

		public Guid? SchoolId {get;set;}


	}
}