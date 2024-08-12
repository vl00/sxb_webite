using System;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;

namespace PMS.OperationPlateform.Domain.Entitys
{
	[Serializable]
	[Table("SysCommonFieldType")]
	public partial class SysCommonFieldType
	{
		[Key]  
		public int Id {get;set;}

		public string CommonFiledTableFieldName {get;set;}

		public string CommonFieldTableName {get;set;}

		public string TypeName {get;set;}


	}
}