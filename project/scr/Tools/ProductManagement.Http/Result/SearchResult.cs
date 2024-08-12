using System;
using System.Collections.Generic;
using System.Text;

namespace ProductManagement.API.Http.Model
{
    /// <summary>
    /// 模糊筛选结果
    /// </summary>
    public class SearchResult
    {
        public int status { get; set; }
        public List<item> items { get; set; }
        public pageinfo pageinfo { get; set; }
    }

    /// <summary>
    /// 学校数据
    /// </summary>
    public class item
    {
        /// <summary>
        /// 学校id
        /// </summary>
        public Guid id { get; set; }
        /// <summary>
        /// 分部id
        /// </summary>
        public Guid eid { get; set; }
        /// <summary>
        /// 分部学校名称、学校名称完全拼接
        /// </summary>
        public string name { get; set; }
    }

    /// <summary>
    /// 当前页数据统计结果
    /// </summary>
    public class pageinfo
    {
        public int curpage { get; set; }
        public int totalpage { get; set; }
        public int countperpage { get; set; }
        public int totalrows { get; set; }
    }
}
