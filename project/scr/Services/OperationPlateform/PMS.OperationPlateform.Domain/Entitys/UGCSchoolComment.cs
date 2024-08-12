using System;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;

namespace PMS.OperationPlateform.Domain.Entitys
{
	[Serializable]
	[Table("UGCSchoolComment")]
	public partial class UGCSchoolComment
	{
		[Key]  
		public int Id {get;set;}

		public Guid? SchoolId {get;set;}

		public int? CommentTypeId {get;set;}

		public string CommentContent {get;set;}

		public Guid? UserId {get;set;}

		public DateTime? CreateTime {get;set;}

		public string Creator {get;set;}


	}
}