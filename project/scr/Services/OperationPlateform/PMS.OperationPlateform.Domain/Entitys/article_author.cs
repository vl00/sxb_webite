using System;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;

namespace PMS.OperationPlateform.Domain.Entitys
{
	[Serializable]
	[Table("article_author")]
	public partial class article_author
	{
		[ExplicitKey]
		public string author {get;set;}

		public string logoUrl {get;set;}


	}
}