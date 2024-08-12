using System;
using Dapper.Contrib.Extensions;

namespace PMS.PaidQA.Domain.Entities
{
	[Serializable]
	[Table("TalentRecord")]
	public partial class TalentRecord
	{
		 /// <summary> 
		 /// </summary> 
		[ExplicitKey]
		public Guid UserId {get;set;}

		 /// <summary> 
		 /// 擅长领域描述 
		 /// </summary> 
		public string RegionDesc {get;set;}

		 /// <summary> 
		 /// 达人简介 
		 /// </summary> 
		public string TalentIntro {get;set;}

		 /// <summary> 
		 /// </summary> 
		public DateTime? CreateTime {get;set;}


	}
}