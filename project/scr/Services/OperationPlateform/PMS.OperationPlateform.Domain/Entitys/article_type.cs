using System;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;

namespace PMS.OperationPlateform.Domain.Entitys
{
	[Serializable]
	[Table("article_type")]
	public partial class article_type
	{
		[ExplicitKey]
		public Guid id {get;set;}

		public string name {get;set;}

		public byte weight {get;set;}


	}
}