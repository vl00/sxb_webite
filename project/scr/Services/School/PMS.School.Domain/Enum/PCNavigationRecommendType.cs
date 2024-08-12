using System.ComponentModel;

namespace PMS.School.Domain.Enum
{
    public enum PCNavigationRecommendType
    {
        /// <summary>
        /// 
        /// </summary>
        [Description("")]
        Unknow = 0,
        /// <summary>
        /// 学校
        /// </summary>
        [Description("学校")]
        School = 1,
        /// <summary>
        /// 图片
        /// </summary>
        [Description("图片")]
        Image = 2,
        /// <summary>
        /// 课程
        /// </summary>
        [Description("课程")]
        Lesson = 3,
        /// <summary>
        /// 中职
        /// </summary>
        [Description("中职")]
        SVS = 4,
        /// <summary>
        /// 机构
        /// </summary>
        [Description("机构")]
        ORG = 5
    }
}
