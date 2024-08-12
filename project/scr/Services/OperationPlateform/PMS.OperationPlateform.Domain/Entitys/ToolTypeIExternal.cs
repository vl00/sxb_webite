using System;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;

namespace PMS.OperationPlateform.Domain.Entitys
{
	[Serializable]
	[Table("ToolTypeIExternal")]
	public partial class ToolTypeIExternal
	{
		[Key]  
		public int Id {get;set;}

		public string Name {get;set;}

		public int? ProvinceId {get;set;}

		public int? CityId {get;set;}

		public decimal Sort {get;set;}


	}
}