using System;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;

namespace PMS.OperationPlateform.Domain.Entitys
{
	[Serializable]
	[Table("SchoolType")]
	public partial class SchoolType
	{
		[Key]  
		public int Id {get;set;}

		public byte Grade {get;set;}

		public int? Type {get;set;}

		public bool? Discount {get;set;}

		public bool? Diglossia {get;set;}

		public bool? Chinese {get;set;}

		public string Name {get;set;}


	}
}