using System;
using System.Collections.Generic;
using System.Text;

namespace ProductManagement.Infrastructure.Toolibrary
{
    public class PageResult<T>
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        /// <summary>
        /// 页码数
        /// </summary>
        public int PageIndex { get; set; }
        /// <summary>
        /// 总条数
        /// </summary>
        public long TotalCount { get; set; }
        /// <summary>
        /// 每页条数
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// 总页数
        /// </summary>
        public long total { get; set; }
        /// <summary>
        /// 数据集合，与bootstrap-table数据绑定字段保持一致，插件直接获取该字段数据
        /// </summary>
        public T rows { get; set; }
    }
}
