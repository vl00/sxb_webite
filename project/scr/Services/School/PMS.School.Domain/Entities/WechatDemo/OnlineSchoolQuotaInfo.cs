using Dapper.Contrib.Extensions;
using Newtonsoft.Json;
using System;

namespace PMS.School.Domain.Entities.WechatDemo
{
    /// <summary>
    /// 指标分配
    /// </summary>
    [Serializable]
    [Table(nameof(OnlineSchoolQuotaInfo))]
    public class OnlineSchoolQuotaInfo
    {
        [ExplicitKey]
        [JsonIgnore]
        public Guid ID { get; set; }
        [JsonIgnore]
        public Guid EID { get; set; }
        /// <summary>
        /// 年份
        /// </summary>
        public int Year { get; set; }
        /// <summary>
        /// 指标分配类型
        /// <para>1.公办示范性普通高中指标计划分配</para>
        /// <para>2.教育集团示范高中直接指标分配</para>
        /// <para>3.指标到校名额分配</para>
        /// </summary>
        public int Type { get; set; }
        /// <summary>
        /// 指标考生数
        /// </summary>
        public int? ExamineeQuantity { get; set; }
        /// <summary>
        /// 省市指标
        /// </summary>
        public int? ProvinceCityQuantity { get; set; }
        /// <summary>
        /// 区属指标
        /// </summary>
        public int? AreaQuantity { get; set; }
        /// <summary>
        /// 学校指标
        /// </summary>
        [JsonIgnore]
        public string SchoolData { get; set; }
        [Computed]
        public object SchoolData_Obj
        {
            get
            {
                try
                {
                    return JsonConvert.DeserializeObject<dynamic>(SchoolData);
                }
                catch { }
                return null;
            }
        }
        /// <summary>
        /// 集团核心校
        /// </summary>
        public string CoreSchool { get; set; }
        /// <summary>
        /// 直接分配的集团指标
        /// </summary>
        public int? CoreQuantity { get; set; }
    }
}