using System;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;

namespace PMS.OperationPlateform.Domain.Entitys
{
	[Serializable]
	[Table("SchoolRank")]
	public partial class SchoolRank
	{
		[ExplicitKey]
		public Guid Id {get;set;}

		public string Title {get;set;}

		public string Cover {get;set;}

		public int? Area {get;set;}

		public decimal Rank {get;set;}

		public bool? ToTop {get;set;}

		public string Remark {get;set;}

		public bool? IsShow {get;set;}

		public DateTime? CreateTime {get;set;}

		public string Creator {get;set;}

		public DateTime? UpdateTime {get;set;}

		public string Updator {get;set;}

		public bool IsDel {get;set;}

		public string Intro {get;set;}

		public string DTSource {get;set;}

		[Write(false)]
        public int No { get; set; }


    }
}