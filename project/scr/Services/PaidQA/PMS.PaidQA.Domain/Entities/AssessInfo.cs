using System;
using Dapper.Contrib.Extensions;
using PMS.PaidQA.Domain.Enums;

namespace PMS.PaidQA.Domain.Entities
{
    /// <summary>
    /// 测评
    /// </summary>
    [Serializable]
    [Table("AssessInfo")]
    public class AssessInfo
    {
        /// <summary> 
        /// </summary> 
        [ExplicitKey]
        public Guid ID { get; set; }
        /// <summary>
        /// 测评用户ID
        /// </summary>
        public Guid UserID { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public AssessStatus Status { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public AssessType Type { get; set; }
        /// <summary>
        /// 推荐的学校分部ID
        /// </summary>
        public string RecommendExtID { get; set; }
        /// <summary>
        /// 推荐的达人用户ID
        /// </summary>
        public Guid RecommendTalentUserID { get; set; }
        /// <summary>
        /// 选择的选项短ID
        /// </summary>
        public string SelectedOptionShortIDs { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
}