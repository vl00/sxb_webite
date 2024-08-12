using System.Collections.Generic;

namespace ProductManagement.Infrastructure.Toolibrary
{
    public class PageResultEx<T>
    {
        public PageResultEx()
        {
            Items = new List<T>();
        }

        public PageResultEx(int pageIndex, int pageSize, long total)
        {
            Items = new List<T>();
            PageIndex = pageIndex;
            PageSize = pageSize;
            PageCount = (total / PageSize) + 1;
            Total = total;
        }
        /// <summary>
        /// 页码数
        /// </summary>
        public int PageIndex { get; set; }
        /// <summary>
        /// 总条数
        /// </summary>
        public long Total { get; set; }
        /// <summary>
        /// 每页条数
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// 总页数
        /// </summary>
        public long PageCount { get; set; }
        /// <summary>
        /// 数据集合，与bootstrap-table数据绑定字段保持一致，插件直接获取该字段数据
        /// </summary>
        public IList<T> Items { get; set; }
    }
}
