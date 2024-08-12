using System;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;

namespace PMS.OperationPlateform.Domain.Entitys
{
	[Serializable]
	[Table("Article_SchoolTypes")]
	public partial class Article_SchoolTypes
	{
		[Key]  
		public int Id {get;set;}

		public int? SchoolType {get;set;}

		public int? SchoolGrade {get;set;}

		public bool? Discount {get;set;}

		public bool? Diglossia {get;set;}

		public bool? Chinese {get;set;}

		public string Name {get;set;}


	}
}