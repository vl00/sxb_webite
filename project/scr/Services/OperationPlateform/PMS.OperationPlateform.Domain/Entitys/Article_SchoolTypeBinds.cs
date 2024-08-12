using System;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;

namespace PMS.OperationPlateform.Domain.Entitys
{
	[Serializable]
	[Table("Article_SchoolTypeBinds")]
	public partial class Article_SchoolTypeBinds
	{
		[Key]  
		public int Id {get;set;}

		public Guid? ArticleId {get;set;}

		public int? SchoolTypeId {get;set;}


	}
}