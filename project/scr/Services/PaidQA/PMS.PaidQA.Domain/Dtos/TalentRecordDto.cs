using PMS.PaidQA.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.PaidQA.Domain.Dtos
{

    /// <summary>
    /// 达人履历基础信息
    /// </summary>
    public class TalentRecordDto:TalentSetting
    {

		/// <summary> 
		/// 擅长领域描述 
		/// </summary> 
		public string RegionDesc { get; set; }

		/// <summary> 
		/// 达人简介 
		/// </summary> 
		public string TalentIntro { get; set; }

		/// <summary> 
		/// </summary> 
		public DateTime? CreateTime { get; set; }

	}
}
