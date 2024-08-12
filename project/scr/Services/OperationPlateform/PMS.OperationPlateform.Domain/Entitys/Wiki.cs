using System;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;

namespace PMS.OperationPlateform.Domain.Entitys
{
	[Serializable]
	[Table("Wiki")]
	public partial class Wiki
	{
		[Key]  
		public Guid Id {get;set;}

		/// <summary>
		/// 词条
		/// </summary>
		public string Name {get;set;}

		/// <summary>
		/// 百科内容
		/// </summary>
		public string Content {get;set;}
	}
}