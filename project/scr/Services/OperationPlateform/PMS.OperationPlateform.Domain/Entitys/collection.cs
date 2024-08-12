using System;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;

namespace PMS.OperationPlateform.Domain.Entitys
{
	[Serializable]
	[Table("collection")]
	public partial class collection
	{
		public Guid? userID {get;set;}

		public byte type {get;set;}

		public Guid? collectionID {get;set;}

		public DateTime? time {get;set;}


	}
}