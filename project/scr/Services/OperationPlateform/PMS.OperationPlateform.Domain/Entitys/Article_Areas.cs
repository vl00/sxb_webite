using System;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;

namespace PMS.OperationPlateform.Domain.Entitys
{
	[Serializable]
	[Table("Article_Areas")]
	public partial class Article_Areas
	{
		[Key]  
		public int Id {get;set;}

		public string ProvinceId {get;set;}

		public string CityId {get;set;}

		public string AreaId {get;set;}

		public Guid? ArticleId {get;set;}


	}
}