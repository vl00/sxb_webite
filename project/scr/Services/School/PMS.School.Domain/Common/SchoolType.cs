using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace PMS.School.Domain.Common
{
    /// <summary>
    /// 学校类型 0 暂未收录 1 公办 2 民办 3 国际 4 外籍 80 港澳台 99 其它
    /// </summary>
    public enum SchoolType : int
    {
        [Description("")]
        unknown = 0,

        /// <summary>
        /// 公办
        /// </summary>
        [Description("公办")]
        Public = 1,
        /// <summary>
        /// 民办
        /// </summary>
        [Description("民办")]
        Private = 2,
        /// <summary>
        /// 国际
        /// </summary>
        [Description("国际")]
        International = 3,
        /// <summary>
        /// 外籍
        /// </summary>
        [Description("外籍")]
        ForeignNationality = 4,
        /// <summary>
        /// 港澳台
        /// </summary>
        [Description("港澳台")]
        SAR = 80,
        /// <summary>
        /// 其它
        /// </summary>
        [Description("其它")]
        Other = 99
    }
}
