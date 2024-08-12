using Dapper.Contrib.Extensions;
using Newtonsoft.Json;
using System;

namespace PMS.School.Domain.Entities.WechatDemo
{
    /// <summary>
    /// 区域招生政策
    /// </summary>
    [Serializable]
    [Table(nameof(SchoolFractionInfo2))]
    public class SchoolFractionInfo2
    {
        [JsonIgnore]
        [ExplicitKey]
        public Guid ID { get; set; }
        [JsonIgnore]
        public Guid EID { get; set; }
        /// <summary>
        /// 类型
        /// <para>1.中考分数线</para>
        /// <para>2.中考录取分数线</para>
        /// <para>3.分数线</para>
        /// </summary>
        public int Type { get; set; }
        /// <summary>
        /// 年份
        /// </summary>
        public int Year { get; set; }
        [JsonIgnore]
        public string Data { get; set; }
        [Computed]
        public object Data_Obj
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(Data))
                {
                    try
                    {
                        return JsonConvert.DeserializeObject<dynamic>(Data);
                    }
                    catch { }
                }
                return null;
            }
        }
    }
}