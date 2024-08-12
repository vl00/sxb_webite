using System;
using Dapper.Contrib.Extensions;

namespace PMS.PaidQA.Domain.Entities
{
	[Serializable]
	[Table("TalentSetting")]
	public partial class TalentSetting
	{
		 /// <summary> 
		 /// </summary> 
		[ExplicitKey]
		public Guid TalentUserID { get;set;}

		 /// <summary> 
		 /// </summary> 
		public bool IsEnable {get;set;}

		 /// <summary> 
		 /// 咨询价格 
		 /// </summary> 
		public decimal Price {get;set;}

		/// <summary>
		/// 达人认证等级ID
		/// </summary>
		public Guid TalentLevelTypeID { get; set; }

        public string JA_Covers { get; set; }
    }
}