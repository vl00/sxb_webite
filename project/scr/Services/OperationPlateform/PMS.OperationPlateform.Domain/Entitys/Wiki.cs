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
		/// ����
		/// </summary>
		public string Name {get;set;}

		/// <summary>
		/// �ٿ�����
		/// </summary>
		public string Content {get;set;}
	}
}