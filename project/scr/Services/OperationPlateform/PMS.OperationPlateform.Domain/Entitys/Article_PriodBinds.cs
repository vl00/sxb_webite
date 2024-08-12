using System;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;

namespace PMS.OperationPlateform.Domain.Entitys
{
	[Serializable]
	[Table("Article_PriodBinds")]
	public partial class Article_PriodBinds
	{
		[Key]  
		public int Id {get;set;}

		public Guid? ArticleId {get;set;}

		public string PriodId {get;set;}


	}
}