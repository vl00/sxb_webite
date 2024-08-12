using PMS.School.Domain.Enum;
using System;

namespace PMS.School.Domain.Entities
{
    /// <summary>
    /// 导航分类
    /// </summary>
    public class PCNavigationInfo
    {
        public Guid ID { get; set; }
        /// <summary>
        /// 分类名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public PCNavigationType Type { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// 图标URL
        /// </summary>
        public string IconUrl { get; set; }
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool Enabled { get; set; }
    }
}
