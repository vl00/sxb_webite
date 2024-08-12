using System;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;

namespace PMS.OperationPlateform.Domain.Entitys
{
	[Serializable]
	[Table("SysCommonFieldTable1")]
	public partial class SysCommonFieldTable1
	{
		[Key]  
		public int Id {get;set;}

		public string DiffId {get;set;}

		public string Field1 {get;set;}

		public string Field2 {get;set;}

		public string Field3 {get;set;}

		public int? Field4 {get;set;}

		public Guid? Field5 {get;set;}

		public string Field6 {get;set;}


	}
}