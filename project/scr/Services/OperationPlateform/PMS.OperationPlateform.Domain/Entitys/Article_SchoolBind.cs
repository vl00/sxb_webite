using System;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;

namespace PMS.OperationPlateform.Domain.Entitys
{
	[Serializable]
	[Table("Article_SchoolBind")]
	public partial class Article_SchoolBind
	{
		[Key]  
		public int Id {get;set;}

		public Guid? ArticleId {get;set;}

		public Guid? SchoolId {get;set;}

		public string SchoolNameCache {get;set;}

		public bool SchoolStatu {get;set;}


	}
}