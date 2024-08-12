using System;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;

namespace PMS.OperationPlateform.Domain.Entitys
{
	[Serializable]
	[Table("ToolTypes")]
	public partial class ToolTypes
	{
		[Key]  
		public int Id {get;set;}

		public string Name {get;set;}

		public byte ParentId {get;set;}

		public string Icon {get;set;}

		public int Sort {get;set;}


	}
}