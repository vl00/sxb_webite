using Dapper.Contrib.Extensions;
using System;

namespace PMS.PaidQA.Domain.Entities
{
    /// <summary>
    /// ѡ���ϵ
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
        /// ��һѡ��(����)ID
        /// </summary>
        public Guid FirstOptionID { get; set; }
        /// <summary>
        /// �ڶ�ѡ��ID
        /// </summary>
        public Guid SecondOptionID { get; set; }
        /// <summary>
        /// ��Ӧѡ���IDs
        /// </summary>
        public string NextOptionShortIDs { get; set; }
        /// <summary>
        /// ����ID
        /// </summary>
        public Guid QuestionID { get; set; }
    }
}