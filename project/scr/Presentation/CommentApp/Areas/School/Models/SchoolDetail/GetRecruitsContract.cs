using PMS.School.Domain.Entities.WechatDemo;
using System.Collections.Generic;

namespace Sxb.Web.Areas.School.Models.SchoolDetail
{
    public class GetRecruitsResponse
    {
        public IEnumerable<OnlineSchoolRecruitInfo> Recruits { get; set; }
        /// <summary>
        /// 申请费用
        /// </summary>
        public string ApplyCost { get; set; }
        /// <summary>
        /// 学费
        /// </summary>
        public string Tuition { get; set; }
        /// <summary>
        /// 其他费用
        /// </summary>
        public string OtherCost { get; set; }
        /// <summary>
        /// 本区招生政策
        /// </summary>
        public string AreaRecruitPlan { get; set; }
        /// <summary>
        /// 各个Type的年份
        /// </summary>
        public IEnumerable<KeyValuePair<int, IEnumerable<int>>> Years { get; set; }
    }
}
