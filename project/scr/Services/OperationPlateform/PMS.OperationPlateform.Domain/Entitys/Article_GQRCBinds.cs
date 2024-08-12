using System;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;

namespace PMS.OperationPlateform.Domain.Entitys
{
	[Serializable]
	[Table("Article_GQRCBinds")]
	public partial class Article_GQRCBinds
	{
		[Key]  
		public int Id {get;set;}

		public Guid? ArticleId {get;set;}

		public int? GQRCId {get;set;}


	}
}