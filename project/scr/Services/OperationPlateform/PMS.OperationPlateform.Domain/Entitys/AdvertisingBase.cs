using System;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;

namespace PMS.OperationPlateform.Domain.Entitys
{
	[Serializable]
	[Table("AdvertisingBase")]
	public partial class AdvertisingBase
	{
		[Key]  
		public int Id {get;set;}

		public string Title {get;set;}

		public string Url {get;set;}

		public string PicUrl {get;set;}

        public int Width { get; set; }

        public int Height { get; set; }

        public bool IsDelete { get; set; }

        public DateTime? CreateTime {get;set;}

		public string Creator {get;set;}

		public DateTime? UpdateTime {get;set;}

		public string Updator {get;set;}


	}
}