using System;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;

namespace PMS.OperationPlateform.Domain.Entitys
{
	[Serializable]
	[Table("CommonNameValue")]
	public partial class CommonNameValue
	{
		[Key]  
		public int Id {get;set;}

		public string Name {get;set;}

		public string Value {get;set;}

		public string Remark {get;set;}

		public DateTime? CreateTime {get;set;}

		public string Creator {get;set;}


	}
}