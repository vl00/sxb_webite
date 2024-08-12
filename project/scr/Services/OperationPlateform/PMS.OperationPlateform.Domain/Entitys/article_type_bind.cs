using System;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;

namespace PMS.OperationPlateform.Domain.Entitys
{
	[Serializable]
	[Table("article_type_bind")]
	public partial class article_type_bind
	{
		public Guid? article_id {get;set;}

		public Guid? type_id {get;set;}


	}
}