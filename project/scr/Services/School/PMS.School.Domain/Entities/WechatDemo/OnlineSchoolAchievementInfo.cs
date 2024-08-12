using Dapper.Contrib.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace PMS.School.Domain.Entities.WechatDemo
{
    /// <summary>
    /// 升学成绩
    /// </summary>
    [Serializable]
    [Table(nameof(OnlineSchoolAchievementInfo))]
    public class OnlineSchoolAchievementInfo
    {
        [ExplicitKey]
        public Guid ID { get; set; }
        public Guid EID { get; set; }
        /// <summary>
        /// 年份
        /// </summary>
        public int Year { get; set; }
        /// <summary>
        /// 平均分
        /// </summary>
        public int? Avg { get; set; }
        /// <summary>
        /// 700+人数/比例
        /// </summary>
        public string Percent700 { get; set; }
        /// <summary>
        /// 750+人数/比例
        /// </summary>
        public string Percent750 { get; set; }
        /// <summary>
        /// 高保线比例
        /// </summary>
        public string PercentHigh { get; set; }
        /// <summary>
        /// 高保线人数
        /// </summary>
        public int? CountHigh { get; set; }
        /// <summary>
        /// 提前批比例
        /// </summary>
        public string PercentAdvance { get; set; }
        /// <summary>
        /// 提前批人数
        /// </summary>
        public int? CountAdvance { get; set; }
        /// <summary>
        /// 高/中考喜报
        /// </summary>
        public string News { get; set; }
        /// <summary>
        /// 毕业生去向
        /// </summary>
        [JsonIgnore]
        public string Forward { get; set; }
        [Computed]
        public object Forward_Obj
        {
            get
            {
                try
                {
                    return JsonConvert.DeserializeObject<dynamic>(Forward);
                }
                catch { }
                return null;
            }
        }
        /// <summary>
        /// 毕业生去向类型
        /// <para>1.高中</para>
        /// <para>2.国内港澳大学</para>
        /// <para>3.国外大学</para>
        /// </summary>
        public int? ForwardType { get; set; }
        /// <summary>
        /// 重本率
        /// </summary>
        public string PercentZB { get; set; }
        /// <summary>
        /// 高优率
        /// </summary>
        public string PercentGY { get; set; }
        /// <summary>
        /// 本科率
        /// </summary>
        public string PercentBK { get; set; }
        /// <summary>
        /// 最高分
        /// </summary>
        public int? Max { get; set; }
        /// <summary>
        /// 梯度数据
        /// </summary>
        [JsonIgnore]
        public string LevelData { get; set; }
        [Computed]
        public IEnumerable<string[]> LevelData_Obj
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(LevelData))
                {
                    try
                    {
                        return JsonConvert.DeserializeObject<IEnumerable<string[]>>(LevelData);
                    }
                    catch { }
                }
                return null;
            }
        }
        /// <summary>
        /// 分数段数据
        /// </summary>
        [JsonIgnore]
        public string FractionData { get; set; }
        [Computed]
        public IEnumerable<string[]> FractionData_Obj
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(FractionData))
                {
                    try
                    {
                        return JsonConvert.DeserializeObject<IEnumerable<string[]>>(FractionData);
                    }

                    catch { }
                }
                return null;
            }
        }
        /// <summary>
        /// 学科数据
        /// </summary>
        [JsonIgnore]
        public string SubjectData { get; set; }
        [Computed]
        public IEnumerable<string[]> SubjectData_Obj
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(SubjectData))
                {
                    try
                    {
                        return JsonConvert.DeserializeObject<IEnumerable<string[]>>(SubjectData);
                    }
                    catch { }
                }
                return null;
            }
        }
        /// <summary>
        /// 重点率
        /// </summary>
        public string PercentZD { get; set; }
        /// <summary>
        /// 700+人数
        /// </summary>
        public int? Count700 { get; set; }
        /// <summary>
        /// 联招上线率
        /// </summary>
        public string PercentLZ { get; set; }
        /// <summary>
        /// 联招线上线人数
        /// </summary>
        public int? CountLZ { get; set; }
        /// <summary>
        /// 一本率
        /// </summary>
        public string PercentYB { get; set; }
        /// <summary>
        /// 文科最高分
        /// </summary>
        public int? MaxWK { get; set; }
        /// <summary>
        /// 理科最高分
        /// </summary>
        public int? MaxLK { get; set; }
    }
}