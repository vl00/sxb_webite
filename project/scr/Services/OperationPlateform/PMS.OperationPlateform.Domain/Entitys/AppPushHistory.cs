using System;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;

namespace PMS.OperationPlateform.Domain.Entitys
{
	[Serializable]
	[Table("AppPushHistory")]
	public partial class AppPushHistory
	{
		[ExplicitKey]
		public Guid Id {get;set;}

		public string Title {get;set;}

		public string Content {get;set;}

		public string Areas {get;set;}

		public string SchoolTypes {get;set;}

		public DateTime? Time {get;set;}

		public DateTime? CreateTime {get;set;}

		public string Creator {get;set;}

		public DateTime? UpdateTime {get;set;}

		public string Updator {get;set;}

		public string Url {get;set;}

		public int Status {get;set;}

		public bool IsDel {get;set;}


	}
}