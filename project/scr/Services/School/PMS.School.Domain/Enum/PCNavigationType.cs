using System.ComponentModel;

namespace PMS.School.Domain.Enum
{
    public enum PCNavigationType
    {
        /// <summary>
        /// 
        /// </summary>
        [Description("")]
        Unkonw = 0,
        /// <summary>
        /// 顶部导航
        /// </summary>
        [Description("顶部导航")]
        HeadNavigation,
        /// <summary>
        /// 主体导航
        /// </summary>
        [Description("主体导航")]
        BodyNatigation
    }
}
