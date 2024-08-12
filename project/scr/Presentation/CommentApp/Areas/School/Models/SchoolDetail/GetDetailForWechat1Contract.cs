using Newtonsoft.Json;
using PMS.PaidQA.Domain.EntityExtend;
using PMS.School.Domain.Dtos;
using PMS.School.Domain.Entities.WechatDemo;
using Sxb.Web.ViewModels.Article;
using Sxb.Web.ViewModels.ViewComponent;
using System;
using System.Collections.Generic;

namespace Sxb.Web.Areas.School.Models.SchoolDetail
{
    public class GetDetailForWechat1Response
    {
        /// <summary>
        /// 开设的项目与课程
        /// </summary>
        public IEnumerable<OnlineSchoolProjectInfo> Projects { get; set; }
        /// <summary>
        /// 课程
        /// </summary>
        public IEnumerable<string> Courses { get; set; }
        /// <summary>
        /// 课程特色
        /// </summary>
        public string CourseCharacteristic { get; set; }
        /// <summary>
        /// 课程认证
        /// </summary>
        public IEnumerable<string> CourseAuths { get; set; }
        public string SchoolName { get; set; }
        public string SchoolExtName { get; set; }
        /// <summary>
        /// 学校认证
        /// </summary>
        public object SchoolAuths { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public int Grade { get; set; }
        /// <summary>
        /// 学校概况
        /// </summary>
        public SchoolOverviewEx Overview { get; set; }
        /// <summary>
        /// 招生信息
        /// </summary>
        public IEnumerable<Recruit> Recruits { get; set; }
        public IEnumerable<KeyValuePair<int, IEnumerable<int>>> RecruitYears { get; set; }
        /// <summary>
        /// 对口学校
        /// </summary>
        public IEnumerable<(string, string, string)> CounterParts { get; set; }
        public IEnumerable<string> CounterPartYears { get; set; }
        /// <summary>
        /// 指标分配
        /// </summary>
        public IEnumerable<OnlineSchoolQuotaInfo> Quotas { get; set; }
        public IEnumerable<KeyValuePair<int, IEnumerable<int>>> QuotaYears { get; set; }
        /// <summary>
        /// 分数线
        /// </summary>
        [JsonIgnore]
        public IEnumerable<OnlineSchoolFractionInfo> Fractions { get; set; }
        public IEnumerable<SchoolFractionInfo2> Fractions2 { get; set; }
        public object Fractions_Obj { get; set; }
        public IEnumerable<int> FractionYears { get; set; }
        public IEnumerable<KeyValuePair<int, IEnumerable<int>>> Fraction2Years { get; set; }
        /// <summary>
        /// 升学成绩
        /// </summary>
        public IEnumerable<OnlineSchoolAchievementInfo> Achievements { get; set; }
        public IEnumerable<int> AchievemenYears { get; set; }
        /// <summary>
        /// 学校图册
        /// </summary>
        public dynamic Images { get; set; }
        /// <summary>
        /// 学校视频
        /// </summary>
        public IEnumerable<KeyValueDto<DateTime, string, byte>> Videos { get; set; }
        /// <summary>
        /// 推荐达人
        /// </summary>
        public TalentDetailExtend Talent { get; set; }
        /// <summary>
        /// 其他分部
        /// </summary>
        public IEnumerable<KeyValueDto<Guid>> OtherExt { get; set; }
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
        [JsonIgnore]
        public string OtherCost { get; set; }
        public object OtherCost_Obj
        {
            get
            {
                try
                {
                    return JsonConvert.DeserializeObject<dynamic>(OtherCost);
                }
                catch { }
                return null;
            }
        }
        public IEnumerable<int> CostYears { get; set; }
        /// <summary>
        /// 本区招生政策
        /// </summary>
        [JsonIgnore]
        public string AreaRecruitPlan { get; set; }
        public object AreaRecruitPlan_Obj
        {
            get
            {
                try
                {
                    return JsonConvert.DeserializeObject<dynamic>(AreaRecruitPlan);
                }
                catch { }
                return null;
            }
        }
        /// <summary>
        /// 学校动态
        /// </summary>
        public IEnumerable<ArticleListItemViewModel> ExtArticles { get; set; }
        /// <summary>
        /// 推荐课程/机构
        /// </summary>
        public OrgLessonViewModel OrgLessons { get; set; }
        /// <summary>
        /// 区域招生政策
        /// </summary>
        public IEnumerable<AreaRecruitPlanInfo> AreaRecruitPlans { get; set; }
        public IEnumerable<int> AreaRecruitPlanYears { get; set; }
        public IEnumerable<dynamic> RecommendExtensions { get; set; }
    }

    public class SchoolOverviewEx : SchoolOverview
    {

    }
}
