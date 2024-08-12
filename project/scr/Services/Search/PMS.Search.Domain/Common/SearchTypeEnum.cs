using System;
using System.ComponentModel;

namespace PMS.Search.Domain.Common
{
    public enum SearchTypeEnum
    {
        /// <summary>
        /// 默认
        /// </summary>
        [Description("默认")]
        defaultType = 0,
        /// <summary>
        /// 学校
        /// </summary>
        [Description("学校")]
        School = 1,
        /// <summary>
        /// 点评
        /// </summary>
        [Description("点评")]
        Comment = 2,
        /// <summary>
        /// 问题
        /// </summary>
        [Description("问题")]
        Question = 3,
        /// <summary>
        /// 回答
        /// </summary>
        [Description("回答")]
        Answer =6,
        /// <summary>
        /// 排行榜
        /// </summary>
        [Description("排行榜")]
        Rank = 4,
        /// <summary>
        /// 排行榜
        /// </summary>
        [Description("文章")]
        Article = 5
    }
}
