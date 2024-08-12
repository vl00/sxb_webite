using System;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;

namespace PMS.OperationPlateform.Domain.Entitys
{
	[Serializable]
	[Table("View_SchoolRankTypeBinds")]
	public partial class View_SchoolRankTypeBinds
	{
		public Guid? SchoolRankId {get;set;}

		public int Id {get;set;}

		public int? SchoolType {get;set;}

		public int? SchoolGrade {get;set;}

		public bool? Discount {get;set;}

		public bool? Diglossia {get;set;}

		public bool? Chinese {get;set;}

		public string Name {get;set;}


	}
}