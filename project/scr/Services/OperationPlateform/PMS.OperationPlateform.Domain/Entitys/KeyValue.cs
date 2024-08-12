using System;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;

namespace PMS.OperationPlateform.Domain.Entitys
{
	[Serializable]
	[Table("KeyValue")]
	public partial class KeyValue
	{
		[ExplicitKey]
		public string Key {get;set;}

		public string Value {get;set;}

		public DateTime? CreateTime {get;set;}

		public string Creator {get;set;}


	}
}