using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace PMS.CommentsManage.Domain.Common
{
    public enum QueryCondition : int
    {
        /// <summary>
        /// 全部
        /// </summary>
        [Description("全部")]
        All = 1,
        /// <summary>
        /// 精华
        /// </summary>
        [Description("精华")]
        Selected = 2,
        /// <summary>
        /// 过来人
        /// </summary>
        [Description("过来人")]
        IsAttend = 3,
        /// <summary>
        /// 辟谣
        /// </summary>
        [Description("辟谣")]
        Rumor = 4,
        /// <summary>
        /// 好评
        /// </summary>
        [Description("好评")]
        IsGood = 5,
        /// <summary>
        /// 差评
        /// </summary>
        [Description("差评")]
        IsBad = 6,
        /// <summary>
        /// 有图
        /// </summary>
        [Description("有图")]
        IsImage = 7,
        /// <summary>
        /// 根据分部查询
        /// </summary>
        [Description("其他分部名称")]
        Other = 8
    }
}
