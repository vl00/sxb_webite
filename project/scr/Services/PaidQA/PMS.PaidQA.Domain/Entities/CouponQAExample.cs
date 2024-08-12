using System;
using Dapper.Contrib.Extensions;

namespace PMS.PaidQA.Domain.Entities
{
	[Serializable]
	[Table("CouponQAExample")]
	public partial class CouponQAExample
	{
		 /// <summary> 
		 /// </summary> 
		[ExplicitKey]
		public Guid Id {get;set;}

		 /// <summary> 
		 /// </summary> 
		public string Question {get;set;}

		 /// <summary> 
		 /// </summary> 
		public string Anwser {get;set;}

		 /// <summary> 
		 /// </summary> 
		public Guid ActivityId { get;set;}

		 /// <summary> 
		 /// </summary> 
		public Guid? TalentUserId {get;set;}

		 /// <summary> 
		 /// </summary> 
		public decimal Sort {get;set;}



    }
}