using System;
using System.Collections.Generic;

namespace Sxb.Web.Areas.PaidQA.Models.Talent
{
    public class GetSettingResponse
    {
        /// <summary>
        /// 是否开启上学问
        /// </summary>
        public bool IsEnable { get; set; }
        /// <summary>
        /// 价格
        /// </summary>
        public string Price { get; set; }
        /// <summary>
        /// 学段IDs
        /// </summary>
        public IEnumerable<Guid> GradeIDs { get; set; }
        /// <summary>
        /// 擅长领域IDs
        /// </summary>
        public IEnumerable<Guid> RegionTypeIDs { get; set; }
        /// <summary>
        /// 可设置最小金额
        /// </summary>
        public string PriceMin { get; set; }
        /// <summary>
        /// 可设置最大金额
        /// </summary>
        public string PriceMax { get; set; }
        public int TalentType { get; set; }
    }
}
