using System;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;

namespace PMS.OperationPlateform.Domain.Entitys
{
	[Serializable]
	[Table("AdvertisingOption")]
	public partial class AdvertisingOption
	{
		[Key]  
		public int Id {get;set;}

		public byte Location {get;set;}

		public double Rate {get;set;}

		public bool? Show {get;set;}

		public DateTime UpTime {get;set;}

		public DateTime ExpireTime {get;set;}

		public DateTime? CreateTime {get;set;}

		public string Creator {get;set;}

		public DateTime? UpdateTime {get;set;}

		public string Updator {get;set;}

		public int ADID {get;set;}

		public int Sort {get;set;}


	}
}