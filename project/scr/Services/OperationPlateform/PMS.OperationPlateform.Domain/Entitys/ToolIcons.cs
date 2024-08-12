using System;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;

namespace PMS.OperationPlateform.Domain.Entitys
{
	[Serializable]
	[Table("ToolIcons")]
	public partial class ToolIcons
	{
		[Key]  
		public int Id {get;set;}

		public string Name {get;set;}

		public string Url {get;set;}

		public string CdnUrl {get;set;}


	}
}