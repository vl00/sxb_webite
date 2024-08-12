using System;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;

namespace PMS.OperationPlateform.Domain.Entitys
{
	[Serializable]
	[Table("Tools")]
	public partial class Tools
	{
		[Key]  
		public int Id {get;set;}

		public string Name {get;set;}

		public int? CityId {get;set;}

		public int? ProvinceId {get;set;}

		public int? ToolTypeId {get;set;}

		public string LinkUrl {get;set;}

		public bool? IsShow {get;set;}


	}
}