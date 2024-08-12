using System;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;

namespace PMS.OperationPlateform.Domain.Entitys
{
	[Serializable]
	[Table("SchoolRankAreaBinds")]
	public partial class SchoolRankAreaBinds
	{
		[Key]  
		public int Id {get;set;}

		public Guid? SchoolRankId {get;set;}

		public int? CityId {get;set;}


	}
}