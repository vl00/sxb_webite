using System;
using System.Collections.Generic;

namespace Sxb.PCWeb.ViewModels.ViewComponent
{
    public class OrgLessonViewModel
    {
        /// <summary>
        /// 推荐机构0
        /// </summary>
        public IEnumerable<RecommendOrg> RecommendOrgs { get; set; }
        /// <summary>
        /// 推荐课程
        /// </summary>
        public IEnumerable<HotSellCourse> HotSellCourses { get; set; }
    }

    public class RecommendOrg
    {
        public Guid ID { get; set; }
        /// <summary>
        /// 短链
        /// </summary>
        public string ID_S { get; set; }
        public string Name { get; set; }
        public string Logo { get; set; }
        public bool Authentication { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Desc { get; set; }
        /// <summary>
        /// 次级描述
        /// </summary>
        public string SubDesc { get; set; }
        /// <summary>
        /// 课程数量
        /// </summary>
        public int CourceCount { get; set; }
        /// <summary>
        /// 测评数量
        /// </summary>
        public int EvaluationCount { get; set; }
    }

    public class HotSellCourse
    {
        public Guid ID { get; set; }
        /// <summary>
        /// 短链
        /// </summary>
        public string ID_S { get; set; }
        public string OrgName { get; set; }
        /// <summary>
        /// 课程名称
        /// </summary>
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public int Subject { get; set; }
        /// <summary>
        /// /课程图片
        /// </summary>
        public string Banner { get; set; }
        /// <summary>
        /// 课程价格
        /// </summary>
        public string Price { get; set; }
        public string OrigPrice { get; set; }
        public bool Authentication { get; set; }
    }
}
