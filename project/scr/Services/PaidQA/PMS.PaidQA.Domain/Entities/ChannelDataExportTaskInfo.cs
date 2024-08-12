using Dapper.Contrib.Extensions;
using System;

namespace PMS.PaidQA.Domain.Entities
{
    /// <summary>
    /// ��Ӫ�������ݵ��������
    /// </summary>
    [Serializable]
    [Table("ChannelDataExportTaskInfo")]
    public class ChannelDataExportTaskInfo
    {
        /// <summary> 
        /// </summary> 
        [ExplicitKey]
        public Guid ID { get; set; }
        /// <summary>
        /// ����ʱ��
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// ��ѯ��ַ
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// ����
        /// </summary>
        public string Channel { get; set; }
        /// <summary>
        /// ͳ�ƿ�ʼʱ��
        /// </summary>
        public DateTime StartTime { get; set; }
        /// <summary>
        /// ͳ�ƽ���ʱ��
        /// </summary>
        public DateTime EndTime { get; set; }
        /// <summary>
        /// �����ļ���URL
        /// </summary>
        public string FileUrl { get; set; }
        /// <summary>
        /// ״̬
        /// <para>0.δ֪</para>
        /// <para>1.������</para>
        /// <para>2.������</para>
        /// <para>3.�ѽ���</para>
        /// </summary>
        public int Status { get; set; }
    }
}