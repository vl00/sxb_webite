using System;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;

namespace PMS.OperationPlateform.Domain.Entitys
{
	[Serializable]
	[Table("SysMenu")]
	public partial class SysMenu
	{
		[Key]  
		public int Id {get;set;}

		public string Name {get;set;}

		public int? Parent {get;set;}

		public int? Sort {get;set;}

		public string Controler {get;set;}

		public string Action {get;set;}

		public string ICon {get;set;}

		public string Url {get;set;}

		public DateTime? CreateTime {get;set;}

		public string Creator {get;set;}


	}
}