using System;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;

namespace PMS.OperationPlateform.Domain.Entitys
{
	[Serializable]
	[Table("SysFuntion")]
	public partial class SysFuntion
	{
		[Key]  
		public int Id {get;set;}

		public string Name {get;set;}

		public int? SysMenuId {get;set;}


	}
}