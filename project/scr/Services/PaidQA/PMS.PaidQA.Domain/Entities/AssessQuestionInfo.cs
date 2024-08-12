using Dapper.Contrib.Extensions;
using PMS.PaidQA.Domain.Enums;
using System;

namespace PMS.PaidQA.Domain.Entities
{
    /// <summary>
    /// 测评选项
    /// </summary>
    [Serializable]
    [Table("AssessQuestionInfo")]
    public class AssessQuestionInfo
    {
        /// <summary> 
        /// </summary> 
        [ExplicitKey]
        public Guid ID { get; set; }
        /// <summary>
        /// 选择类型
        /// </summary>
        public AssessSelectType SelectType { get; set; }
        /// <summary>
        /// 测评类型
        /// </summary>
        public AssessType AssessType { get; set; }
        /// <summary>
        /// 问题层级
        /// </summary>
        public int Level { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 是否必填
        /// </summary>
        public bool IsRequired { get; set; }
    }
}