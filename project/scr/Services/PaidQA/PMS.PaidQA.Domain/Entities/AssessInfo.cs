using System;
using Dapper.Contrib.Extensions;
using PMS.PaidQA.Domain.Enums;

namespace PMS.PaidQA.Domain.Entities
{
    /// <summary>
    /// ����
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
        /// �����û�ID
        /// </summary>
        public Guid UserID { get; set; }
        /// <summary>
        /// ״̬
        /// </summary>
        public AssessStatus Status { get; set; }
        /// <summary>
        /// ����
        /// </summary>
        public AssessType Type { get; set; }
        /// <summary>
        /// �Ƽ���ѧУ�ֲ�ID
        /// </summary>
        public string RecommendExtID { get; set; }
        /// <summary>
        /// �Ƽ��Ĵ����û�ID
        /// </summary>
        public Guid RecommendTalentUserID { get; set; }
        /// <summary>
        /// ѡ���ѡ���ID
        /// </summary>
        public string SelectedOptionShortIDs { get; set; }
        /// <summary>
        /// ����ʱ��
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
}