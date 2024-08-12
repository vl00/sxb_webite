using Dapper.Contrib.Extensions;
using System;

namespace PMS.PaidQA.Domain.Entities
{
    /// <summary>
    /// 选项关系
    /// </summary>
    [Serializable]
    [Table("AssessOptionRelationInfo")]
    public class AssessOptionRelationInfo
    {
        /// <summary> 
        /// </summary> 
        [ExplicitKey]
        public Guid ID { get; set; }
        /// <summary>
        /// 第一选项(问题)ID
        /// </summary>
        public Guid FirstOptionID { get; set; }
        /// <summary>
        /// 第二选项ID
        /// </summary>
        public Guid SecondOptionID { get; set; }
        /// <summary>
        /// 对应选项短IDs
        /// </summary>
        public string NextOptionShortIDs { get; set; }
        /// <summary>
        /// 问题ID
        /// </summary>
        public Guid QuestionID { get; set; }
    }
}