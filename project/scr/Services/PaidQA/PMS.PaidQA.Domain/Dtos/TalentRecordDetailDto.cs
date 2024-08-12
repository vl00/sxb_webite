using PMS.PaidQA.Domain.EntityExtend;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.PaidQA.Domain.Dtos
{
   public class TalentRecordDetailDto: TalentDetailExtend
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
