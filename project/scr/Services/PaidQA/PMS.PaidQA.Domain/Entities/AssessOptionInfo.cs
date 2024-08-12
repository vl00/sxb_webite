using Dapper.Contrib.Extensions;
using System;

namespace PMS.PaidQA.Domain.Entities
{
    /// <summary>
    /// 测评选项
    /// </summary>
    [Serializable]
    [Table("AssessOptionInfo")]
    public class AssessOptionInfo
    {
        /// <summary> 
        /// </summary> 
        [ExplicitKey]
        public Guid ID { get; set; }
        /// <summary>
        /// 短ID
        /// </summary>
        public int ShortID { get; set; }
        /// <summary>
        /// 选项内容
        /// </summary>
        public string Content { get; set; }
    }
}