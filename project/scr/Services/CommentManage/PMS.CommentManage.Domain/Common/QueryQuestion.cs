using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace PMS.CommentsManage.Domain.Common
{
    public enum QueryQuestion : int
    {
        /// <summary>
        /// 全部
        /// </summary>
        [Description("全部")]
        All = 1,
        /// <summary>
        /// 辟谣
        /// </summary>
        [Description("辟谣")]
        Rumor = 2,
        /// <summary>
        /// 师资
        /// </summary>
        [Description("师资")]
        Teacher = 3,
        /// <summary>
        /// 硬件
        /// </summary>
        [Description("硬件")]
        Hard = 4,
        /// <summary>
        /// 环境
        /// </summary>
        [Description("环境")]
        Envir = 5,
        /// <summary>
        /// 学分
        /// </summary>
        [Description("学分")]
        Manage = 6,
        /// <summary>
        /// 校园
        /// </summary>
        [Description("校园")]
        Life = 7,
        /// <summary>
        /// 根据分部查询
        /// </summary>
        [Description("其他分部名称")]
        Other = 8
    }
}
