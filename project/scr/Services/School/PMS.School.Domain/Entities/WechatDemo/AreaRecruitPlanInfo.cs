using Dapper.Contrib.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace PMS.School.Domain.Entities.WechatDemo
{
    /// <summary>
    /// 区域招生政策
    /// </summary>
    [Serializable]
    [Table(nameof(AreaRecruitPlanInfo))]
    public class AreaRecruitPlanInfo
    {
        [JsonIgnore]
        [ExplicitKey]
        public Guid ID { get; set; }
        /// <summary>
        /// 学校类型
        /// </summary>
        [JsonIgnore]
        public string SchFType { get; set; }
        /// <summary>
        /// 区域代号
        /// </summary>
        [JsonIgnore]
        public string AreaCode { get; set; }
        /// <summary>
        /// 链接数据
        /// </summary>
        [JsonIgnore]
        public string UrlData { get; set; }
        [Computed]
        public IEnumerable<dynamic> UrlData_Obj
        {
            get
            {
                try
                {
                    return JsonConvert.DeserializeObject<dynamic>(UrlData);
                }
                catch { }
                return null;
            }
        }
        /// <summary>
        /// 积分办法
        /// </summary>
        public string PointMethod { get; set; }
        /// <summary>
        /// 年份
        /// </summary>
        public int Year { get; set; }
    }
}