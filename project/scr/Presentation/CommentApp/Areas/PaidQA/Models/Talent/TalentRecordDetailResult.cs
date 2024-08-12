using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Web.Areas.PaidQA.Models.Talent
{
    public class TalentRecordDetailResult: GetDetailResponse
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
