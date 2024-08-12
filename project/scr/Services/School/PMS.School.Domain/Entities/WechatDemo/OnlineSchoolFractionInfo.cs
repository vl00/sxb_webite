using Dapper.Contrib.Extensions;
using System;

namespace PMS.School.Domain.Entities.WechatDemo
{
    /// <summary>
    /// 分数线
    /// </summary>
    [Serializable]
    [Table(nameof(OnlineSchoolFractionInfo))]
    public class OnlineSchoolFractionInfo
    {
        [ExplicitKey]
        public Guid ID { get; set; }
        public Guid EID { get; set; }
        /// <summary>
        /// 年份
        /// </summary>
        public int Year { get; set; }
        /// <summary>
        /// 批次
        /// <para>1.独立批</para>
        /// <para>2.提前批统一计划</para>
        /// <para>3.第一批</para>
        /// <para>4.第二批</para>
        /// <para>5.第三批</para>
        /// <para>6.补录</para>
        /// <para>7.第四批</para>
        /// </summary>
        public int? BatchType { get; set; }
        /// <summary>
        /// 计划区域
        /// </summary>
        public string Area { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public string StudentType { get; set; }
        /// <summary>
        /// 录取分数线
        /// </summary>
        public int? Fraction { get; set; }
        /// <summary>
        /// 指标到校录取最低控制分数线
        /// </summary>
        public int? LowestFraction { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int? BatchIndex { get; set; }
    }
}