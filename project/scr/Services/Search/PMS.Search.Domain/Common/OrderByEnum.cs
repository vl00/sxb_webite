using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.Search.Domain.Common
{
    /// <summary>
    /// 搜索排序 0  默认排序(相关度排序) 1 热度逆序 2 热度顺序 3 时间逆序 4 时间顺序
    /// </summary>
    public enum ArticleOrderBy
    {
        /// <summary>
        /// 默认排序(相关度排序)
        /// </summary>
        Default = 0,
        /// <summary>
        /// 热度逆序
        /// </summary>
        HotDesc = 1,
        /// <summary>
        /// 热度顺序
        /// </summary>
        HotAsc = 2,
        /// <summary>
        /// 时间逆序
        /// </summary>
        TimeDesc = 3,
        /// <summary>
        /// 时间顺序
        /// </summary>
        TimeAsc = 4,
        /// <summary>
        /// 置顶时间逆序
        /// </summary>
        TopTimeDesc = 5,
        /// <summary>
        /// 置顶时间顺序
        /// </summary>
        TopTimeAsc = 6,
    }
}
