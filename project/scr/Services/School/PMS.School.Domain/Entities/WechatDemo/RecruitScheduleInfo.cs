using Dapper.Contrib.Extensions;
using Newtonsoft.Json;
using System;

namespace PMS.School.Domain.Entities.WechatDemo
{
    /// <summary>
    /// 招生日程
    /// </summary>
    [Serializable]
    [Table(nameof(RecruitScheduleInfo))]
    public class RecruitScheduleInfo
    {
        [JsonIgnore]
        [ExplicitKey]
        public Guid ID { get; set; }
        /// <summary>
        /// 招生信息ID
        /// </summary>
        [JsonIgnore]
        public Guid RecruitID { get; set; }
        /// <summary>
        /// 招生信息类型
        /// </summary>
        [JsonIgnore]
        public int? RecruitType { get; set; }
        /// <summary>
        /// 城市代码
        /// </summary>
        [JsonIgnore]
        public int? CityCode { get; set; }
        /// <summary>
        /// 区域代码
        /// </summary>
        [JsonIgnore]
        public int? AreaCode { get; set; }
        /// <summary>
        /// 时间
        /// </summary>
        public string StrDate { get; set; }
        /// <summary>
        /// 重要事项内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int? Index { get; set; }
        /// <summary>
        /// 学校类型
        /// </summary>
        [JsonIgnore]
        public string SchFType { get; set; }
    }
}