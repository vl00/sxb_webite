using System;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;

namespace PMS.OperationPlateform.Domain.Entitys
{
	[Serializable]
	[Table("Comparision_Articles")]
	public partial class Comparision_Articles
	{
		public Guid id {get;set;}

		public string title {get;set;}

		public string author {get;set;}

		public string author_origin {get;set;}

		public DateTime? time {get;set;}

		public string url {get;set;}

		public string type {get;set;}

		public string url_origin {get;set;}

		public byte layout {get;set;}

		public string img {get;set;}

		public string overview {get;set;}

		public string html {get;set;}

		public bool toTop {get;set;}

		public bool show {get;set;}

		public bool? linkOnly {get;set;}

		public int? viewCount {get;set;}

		public int? viewCount_r {get;set;}

		public bool? assistant {get;set;}

		public string city {get;set;}

		public Guid? SchoolId {get;set;}


	}
}