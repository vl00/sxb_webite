using Dapper.Contrib.Extensions;
using PMS.PaidQA.Domain.Enums;
using System;

namespace PMS.PaidQA.Domain.Entities
{
    [Serializable]
    [Table("AssessStatisticsInfo")]
    public class AssessStatisticsInfo
    {
        [ExplicitKey]
        public Guid ID { get; set; }
        /// <summary>
        /// 测评类型
        /// </summary>
        public AssessType AssessType { get; set; }
        /// <summary>
        /// 统计日期
        /// </summary>
        public DateTime CountTime { get; set; }
        /// <summary>
        /// 首页访问总UV/日
        /// </summary>
        public long HomeIndexUV { get; set; }
        /// <summary>
        /// 首页访问总PV/日
        /// </summary>
        public long HomeIndexPV { get; set; }
        /// <summary>
        /// 第一题页面跳出率/日
        /// </summary>
        public decimal FirstPagePopoutPercent { get; set; }
        /// <summary>
        /// 完成所有题目总UV/日
        /// </summary>
        public long FinishUV { get; set; }
        /// <summary>
        /// 完成所有题目总PV/日
        /// </summary>
        public long FinishPV { get; set; }
        /// <summary>
        /// 服务号新增关注数/日
        /// </summary>
        public int FollowCount { get; set; }
        /// <summary>
        /// 付款成功订单数/日
        /// </summary>
        public int PayCount { get; set; }
        /// <summary>
        /// 是否已统计
        /// </summary>
        public bool IsCounted { get; set; }
    }
}
