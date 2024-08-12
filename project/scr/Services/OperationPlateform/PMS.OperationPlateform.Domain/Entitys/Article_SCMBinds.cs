using System;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;

namespace PMS.OperationPlateform.Domain.Entitys
{
	[Serializable]
	[Table("Article_SCMBinds")]
	public partial class Article_SCMBinds
	{
		[Key]  
		public int Id {get;set;}

		public Guid? ArticleId {get;set;}

		public string SCMId {get;set;}


	}
}