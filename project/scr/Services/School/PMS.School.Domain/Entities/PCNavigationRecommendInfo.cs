using PMS.School.Domain.Enum;
using System;

namespace PMS.School.Domain.Entities
{
    /// <summary>
    /// 导航推荐
    /// </summary>
    public class PCNavigationRecommendInfo
    {
        public Guid ID { get; set; }
        /// <summary>
        /// 推荐内容(名称)
        /// </summary>
        public string Content { get; set; }
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
        /// 图片URL
        /// </summary>
        public string ImgUrl { get; set; }
        /// <summary>
        /// 对象ID
        /// </summary>
        public Guid DataID { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public PCNavigationRecommendType Type { get; set; }
        /// <summary>
        /// 城市代码
        /// </summary>
        public int City { get; set; }
    }
}
