using Newtonsoft.Json;
using PMS.PaidQA.Domain.EntityExtend;
using PMS.School.Domain.Common;
using PMS.School.Domain.Dtos;
using PMS.School.Domain.Entities.WechatDemo;
using Sxb.Web.Models.Question;
using Sxb.Web.ViewModels.Article;
using Sxb.Web.ViewModels.ViewComponent;
using System;
using System.Collections.Generic;

namespace Sxb.Web.Areas.School.Models.SchoolDetail
{
    public class GetDetailForWechatResponse
    {
        /// <summary>
        /// 学校类型名称
        /// </summary>
        public string TypeName { get; set; }
        /// <summary>
        /// 学段
        /// </summary>
        public int Grade { get; set; }
        /// <summary>
        /// 学校名称
        /// </summary>
        public string SchoolName { get; set; }
        /// <summary>
        /// 学部名称
        /// </summary>
        public string ExtName { get; set; }
        /// <summary>
        /// 纬度
        /// </summary>
        public double? Latitude { get; set; }
        /// <summary>
        /// 经度
        /// </summary>
        public double? Longitude { get; set; }
        /// <summary>
        /// 学校主图Url
        /// </summary>
        public string SchoolImageUrl { get; set; }
        /// <summary>
        /// 学校图片
        /// </summary>
        public List<SchoolImageDto> SchoolImages { get; set; }
        /// <summary>
        /// 学校英文名
        /// </summary>
        public string EnglishName { get; set; }
        /// <summary>
        /// 学校标签
        /// </summary>
        public IList<string> Tags { get; set; }
        /// <summary>
        /// 学校概况
        /// </summary>
        public SchoolOverview Overview { get; set; }
        /// <summary>
        /// 招生信息
        /// </summary>
        public IEnumerable<Recruit> Recruits { get; set; }
        /// <summary>
        /// 对口学校
        /// </summary>
        public IEnumerable<(string, string, string)> CounterPart { get; set; }
        /// <summary>
        /// 指标分配
        /// </summary>
        public IEnumerable<OnlineSchoolQuotaInfo> Quotas { get; set; }
        /// <summary>
        /// 升学成绩
        /// </summary>
        public IEnumerable<OnlineSchoolAchievementInfo> Achievements { get; set; }
        /// <summary>
        /// 分数线
        /// </summary>
        [JsonIgnore]
        public IEnumerable<OnlineSchoolFractionInfo> Fractions { get; set; }
        public IEnumerable<SchoolFractionInfo2> Fractions2 { get; set; }
        public object Fractions_Obj { get; set; }
        /// <summary>
        /// 学校视频
        /// </summary>
        public IEnumerable<KeyValueDto<DateTime, string, byte, string, string>> SchoolVideo { get; set; }
        /// <summary>
        /// 推荐达人
        /// </summary>
        public TalentDetailExtend Talent { get; set; }
        /// <summary>
        /// 家长问答
        /// </summary>
        public QuestionVo Question { get; set; }
        /// <summary>
        /// 学校动态
        /// </summary>
        public IEnumerable<ArticleListItemViewModel> ExtArticles { get; set; }
        /// <summary>
        /// 其他学部
        /// </summary>
        public IEnumerable<KeyValueDto<Guid>> ExtSchools { get; set; }
        /// <summary>
        /// 推荐课程/机构
        /// </summary>
        public OrgLessonViewModel OrgLessons { get; set; }
        /// <summary>
        /// 是否认证
        /// </summary>
        public bool IsAuth { get; set; }

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
        /// <summary>
        /// 本区招生政策
        /// </summary>
        public IEnumerable<AreaRecruitPlanInfo> AreaRecruitPlan { get; set; }
        /// <summary>
        /// 相关推荐
        /// </summary>
        public IEnumerable<dynamic> RecommendExtensions { get; set; }
    }
    public class SchoolOverview
    {
        /// <summary>
        /// 学生数量
        /// </summary>
        public int StudentQuantity { get; set; }
        /// <summary>
        /// 师生比
        /// </summary>
        public string TeacherStudentScale { get; set; }
        /// <summary>
        /// 是否有校车
        /// </summary>
        public bool? HasSchoolBus { get; set; }
        /// <summary>
        /// 是否有饭堂
        /// </summary>
        public bool? HasCanteen { get; set; }
        /// <summary>
        /// 寄宿类型
        /// </summary>
        public LodgingEnum LodgingType { get; set; }
        /// <summary>
        /// 出国方向
        /// </summary>
        [JsonIgnore]
        public string Abroad { get; set; }
        public object Abroad_Obj
        {
            get
            {
                try
                {
                    return JsonConvert.DeserializeObject<dynamic>(Abroad);
                }
                catch { }
                return null;
            }
        }
        /// <summary>
        /// 学校地址
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 电话
        /// </summary>
        public string Tel { get; set; }
        /// <summary>
        /// 官网
        /// </summary>
        public string WebSite { get; set; }
        /// <summary>
        /// 介绍
        /// </summary>
        public string Intro { get; set; }
        /// <summary>
        /// 招生方式
        /// </summary>
        public object RecruitWay { get; set; }
        /// <summary>
        /// 城市代号
        /// </summary>
        public int CityCode { get; set; }
    }
    public class Recruit : OnlineSchoolRecruitInfo
    {
        /// <summary>
        /// 招生日程
        /// </summary>
        public IEnumerable<RecruitScheduleInfo> RecruitSchedules { get; set; }
    }
}
