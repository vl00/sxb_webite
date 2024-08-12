using Dapper.Contrib.Extensions;
using PMS.School.Domain.Enum;
using System;

namespace PMS.School.Domain.Entities.SpecialTopic
{
    /// <summary>
    /// 专题管理
    /// </summary>
    [Table("SpecialTopic")]
    public class SpecialTopic
    {
        [ExplicitKey]
        public Guid ID { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 封面Url
        /// </summary>
        public string CoverUrl { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public SpecialTopicType Type { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// 城市Code
        /// </summary>
        public int CityCode { get; set; }
    }
}
