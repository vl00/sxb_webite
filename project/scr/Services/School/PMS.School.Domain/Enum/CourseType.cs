using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.School.Domain.Enum
{
    public enum CourseType
    {
        /// <summary>
        /// 全部
        /// </summary>
        All = 0,
        /// <summary>
        /// 美国课程
        /// </summary>
        USA = 1,
        /// <summary>
        /// A-Level
        /// </summary>
        ALevel = 2,
        /// <summary>
        /// IB
        /// </summary>
        IB = 3,
        /// <summary>
        /// 加拿大课程
        /// </summary>
        Canada = 4,
        /// <summary>
        /// 澳洲课程
        /// </summary>
        Australia = 5,
        /// <summary>
        /// 其他课程
        /// </summary>
        Other = 6
    }
}
