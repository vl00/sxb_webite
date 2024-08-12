using System;
using System.ComponentModel;

namespace PMS.School.Domain.Common
{
    /// <summary>
    /// 寄宿类型 0 暂未收录 1 走读 2 寄宿 3 寄宿&走读
    /// </summary>
    public enum LodgingEnum
    {
        /// <summary>
        /// 未收录
        /// </summary>
        [Description("暂未收录")]
        Unkown = 0,
        /// <summary>
        /// 走读
        /// </summary>
        [Description("走读")]
        Walking = 1,
        /// <summary>
        /// 寄宿
        /// </summary>
        [Description("寄宿")]
        Lodging = 2,
        /// <summary>
        /// 可走读、寄宿 
        /// </summary>
        [Description("寄宿&走读")]
        LodgingOrGo = 3
    }
}
