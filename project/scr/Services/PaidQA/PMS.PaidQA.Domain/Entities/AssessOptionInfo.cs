using Dapper.Contrib.Extensions;
using System;

namespace PMS.PaidQA.Domain.Entities
{
    /// <summary>
    /// ����ѡ��
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
        /// ��ID
        /// </summary>
        public int ShortID { get; set; }
        /// <summary>
        /// ѡ������
        /// </summary>
        public string Content { get; set; }
    }
}