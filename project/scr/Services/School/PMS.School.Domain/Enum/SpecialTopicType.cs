using System.ComponentModel;

namespace PMS.School.Domain.Enum
{
    /// <summary>
    /// 专题类型
    /// </summary>
    public enum SpecialTopicType
    {
        Unknow = 0,
        /// <summary>
        /// 直播
        /// </summary>
        [Description("直播")]
        Live = 1,
        /// <summary>
        /// 功率(文章)
        /// </summary>
        [Description("攻略(文章)")]
        Article = 2
    }
}
