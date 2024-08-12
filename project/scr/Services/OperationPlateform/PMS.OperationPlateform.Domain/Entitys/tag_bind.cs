using System;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;

namespace PMS.OperationPlateform.Domain.Entitys
{
	[Serializable]
	[Table("tag_bind")]
	public partial class tag_bind
	{
		public Guid? tagID {get;set;}

		public Guid? dataID {get;set;}

		public byte dataType {get;set;}

		public byte dataType_s {get;set;}

		public bool? ms {get;set;}


	}
}