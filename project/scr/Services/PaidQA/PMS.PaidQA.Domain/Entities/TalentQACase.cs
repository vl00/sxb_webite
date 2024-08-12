using System;
using Dapper.Contrib.Extensions;

namespace PMS.PaidQA.Domain.Entities
{
	[Serializable]
	[Table("TalentQACase")]
	public partial class TalentQACase
	{
		 /// <summary> 
		 /// </summary> 
		[ExplicitKey]
		public Guid Id {get;set;}

		 /// <summary> 
		 /// 案例背景 
		 /// </summary> 
		public string Background {get;set;}

		 /// <summary> 
		 /// 案例图片 
		 /// </summary> 
		public string Images {get;set;}


	}
}