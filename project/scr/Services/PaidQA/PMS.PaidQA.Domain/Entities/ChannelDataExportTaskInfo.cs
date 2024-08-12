using Dapper.Contrib.Extensions;
using System;

namespace PMS.PaidQA.Domain.Entities
{
    /// <summary>
    /// 运营渠道数据导出任务表
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
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 查询地址
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// 渠道
        /// </summary>
        public string Channel { get; set; }
        /// <summary>
        /// 统计开始时间
        /// </summary>
        public DateTime StartTime { get; set; }
        /// <summary>
        /// 统计结束时间
        /// </summary>
        public DateTime EndTime { get; set; }
        /// <summary>
        /// 导出文件的URL
        /// </summary>
        public string FileUrl { get; set; }
        /// <summary>
        /// 状态
        /// <para>0.未知</para>
        /// <para>1.待进行</para>
        /// <para>2.进行中</para>
        /// <para>3.已结束</para>
        /// </summary>
        public int Status { get; set; }
    }
}