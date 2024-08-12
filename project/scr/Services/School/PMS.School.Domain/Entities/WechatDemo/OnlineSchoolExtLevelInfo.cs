using Dapper.Contrib.Extensions;
using Newtonsoft.Json;
using System;

namespace PMS.School.Domain.Entities.WechatDemo
{
    /// <summary>
    /// 学校级别(微信Demo用)
    /// </summary>
    [Serializable]
    [Table(nameof(OnlineSchoolExtLevelInfo))]
    public class OnlineSchoolExtLevelInfo
    {
        [ExplicitKey]
        [JsonIgnore]
        public Guid ID { get; set; }
        /// <summary>
        /// 学段
        /// </summary>
        public int? Grade { get; set; }
        /// <summary>
        /// 等级名称
        /// </summary>
        public string LevelName { get; set; }
        /// <summary>
        /// 城市代码
        /// </summary>
        public int? CityCode { get; set; }
        /// <summary>
        /// 须替换级别来源
        /// </summary>
        public string ReplaceSource { get; set; }
        /// <summary>
        /// 学校类型
        /// </summary>
        public string SchFType { get; set; }
    }
}