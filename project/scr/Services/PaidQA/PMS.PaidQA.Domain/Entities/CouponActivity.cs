using System;
using Dapper.Contrib.Extensions;

namespace PMS.PaidQA.Domain.Entities
{
	[Serializable]
	[Table("CouponActivity")]
	public partial class CouponActivity
	{
		 /// <summary> 
		 /// </summary> 
		[ExplicitKey]  
		public Guid Id {get;set;}

		 /// <summary> 
		 /// </summary> 
		public string Title {get;set;}

		 /// <summary> 
		 /// </summary> 
		public DateTime? StartTime {get;set;}

		 /// <summary> 
		 /// </summary> 
		public DateTime? OverTime {get;set;}

		 /// <summary> 
		 /// </summary> 
		public string Creator {get;set;}


        public Guid CouponId { get; set; }


        public string ShareTitle { get; set; }

        public string ShareContent { get; set; }

        public string ShareImg { get; set; }

    }
}