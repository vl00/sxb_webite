using System;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;

namespace PMS.OperationPlateform.Domain.Entitys
{
	[Serializable]
	[Table("Users")]
	public partial class Users
	{
		[Key]  
		public int Id {get;set;}

		public string Name {get;set;}


	}
}