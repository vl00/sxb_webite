using System;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;

namespace PMS.OperationPlateform.Domain.Entitys
{
	[Serializable]
	[Table("GroupQRCode")]
	public partial class GroupQRCode
	{
		[Key]  
		public int Id {get;set;}

		public string Url {get;set;}


	}
}