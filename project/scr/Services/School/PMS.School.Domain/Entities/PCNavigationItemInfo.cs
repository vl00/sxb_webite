using System;

namespace PMS.School.Domain.Entities
{
    /// <summary>
    /// 导航子项
    /// </summary>
    public class PCNavigationItemInfo
    {
        public Guid ID { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// 跳转URL
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// 导航分类ID
        /// </summary>
        public Guid PCNavigationID { get; set; }
        /// <summary
        /// 父级ID
        /// </summary>
        public Guid ParentID { get; set; }

        public int City { get; set; }
    }
}
