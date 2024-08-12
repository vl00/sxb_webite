using Dapper.Contrib.Extensions;
using PMS.PaidQA.Domain.Enums;
using System;

namespace PMS.PaidQA.Domain.Entities
{
    /// <summary>
    /// ����ѡ��
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
        /// ѡ������
        /// </summary>
        public AssessSelectType SelectType { get; set; }
        /// <summary>
        /// ��������
        /// </summary>
        public AssessType AssessType { get; set; }
        /// <summary>
        /// ����㼶
        /// </summary>
        public int Level { get; set; }
        /// <summary>
        /// ����
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// �Ƿ����
        /// </summary>
        public bool IsRequired { get; set; }
    }
}