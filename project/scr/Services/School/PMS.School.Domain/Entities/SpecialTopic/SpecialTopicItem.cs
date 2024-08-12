using Dapper.Contrib.Extensions;
using System;

namespace PMS.School.Domain.Entities.SpecialTopic
{
    /// <summary>
    /// 专题子项
    /// </summary>
    [Table("SpecialTopicItem")]
    public class SpecialTopicItem
    {
        [ExplicitKey]
        public Guid ID { get; set; }
        /// <summary>
        /// 专题ID
        /// </summary>
        public Guid SpecialTopicID { get; set; }
        /// <summary>
        /// 目标主题ID
        /// </summary>
        public Guid TargetID { get; set; }
        /// <summary>
        /// 目标标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 目标达人名称
        /// </summary>
        public string TargetUserName { get; set; }
        /// <summary>
        /// 目标达人ID
        /// </summary>
        public Guid TargetUserID { get; set; }
        /// <summary>
        /// 目标达人头像
        /// </summary>
        public string TargetUserImgUrl { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int Index { get; set; }
    }
}
