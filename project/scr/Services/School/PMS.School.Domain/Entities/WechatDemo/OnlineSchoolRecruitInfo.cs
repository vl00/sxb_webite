using Dapper.Contrib.Extensions;
using System;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace PMS.School.Domain.Entities.WechatDemo
{
    /// <summary>
    /// 招生信息
    /// </summary>
    [Serializable]
    [Table(nameof(OnlineSchoolRecruitInfo))]
    public class OnlineSchoolRecruitInfo
    {
        [ExplicitKey]
        [JsonIgnore]
        public Guid ID { get; set; }
        [JsonIgnore]
        public Guid EID { get; set; }
        /// <summary>
        /// 招生信息类型
        /// <para>1.本校招生信息</para>
        /// <para>2.户籍生招生信息</para>
        /// <para>3.非户籍生积分入学</para>
        /// <para>4.分类招生简章</para>
        /// <para>5.社会公开招生简章</para>
        /// <para>6.自主招生</para>
        /// </summary>
        public int Type { get; set; }
        /// <summary>
        /// 年份
        /// </summary>
        public int Year { get; set; }
        /// <summary>
        /// 最小招生年龄
        /// </summary>
        public string MinAge { get; set; }
        /// <summary>
        /// 最大招生年龄
        /// </summary>
        public string MaxAge { get; set; }
        /// 招生人数
        /// </summary>
        public int? Quantity { get; set; }
        /// <summary>
        /// 招生总计划(人数)
        /// </summary>
        public int? PlanQuantity { get; set; }
        /// <summary>
        /// 指标计划
        /// </summary>
        public int? QuotaPlanQuantity { get; set; }
        /// <summary>
        /// 招生对象
        /// </summary>
        public string Target { get; set; }
        /// <summary>
        /// 招生范围
        /// </summary>
        public string Score { get; set; }
        /// <summary>
        /// 招生比例
        /// </summary>
        public string Scale { get; set; }
        /// <summary>
        /// 招生简章
        /// </summary>
        public string Brief { get; set; }
        /// <summary>
        /// 派位小学
        /// </summary>
        [JsonIgnore]
        public string AllocationPrimary { get; set; }
        [Computed]
        public object AllocationPrimary_Obj
        {
            get
            {
                try
                {
                    return Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(AllocationPrimary);
                }
                catch { }
                return null;
            }
        }
        /// <summary>
        /// 对口小学
        /// </summary>
        [JsonIgnore]
        public string CounterpartPrimary { get; set; }
        [Computed]
        public object CounterpartPrimary_Obj
        {
            get
            {
                try
                {
                    return Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(CounterpartPrimary);
                }
                catch { }
                return null;
            }
        }
        /// <summary>
        /// 对口范围
        /// </summary>
        public string CounterpartScore { get; set; }
        /// <summary>
        /// 派位范围
        /// </summary>
        public string AllocationScore { get; set; }
        /// <summary>
        /// 积分入学指标及分值体系表图片URL
        /// </summary>
        public string IntegralImgUrl { get; set; }
        /// <summary>
        /// 积分入学入围分数线
        /// </summary>
        public int? IntegralPassLevel { get; set; }
        /// <summary>
        /// 积分入学录取分数线
        /// </summary>
        public int? IntegralAdmitLevel { get; set; }
        /// <summary>
        /// 申请费用
        /// </summary>
        public string ApplyCost { get; set; }
        /// <summary>
        /// 学费
        /// </summary>
        public string Tuition { get; set; }
        /// <summary>
        /// 其他费用
        /// </summary>
        [JsonIgnore]
        public string OtherCost { get; set; }
        [Computed]
        public object OtherCost_Obj
        {
            get
            {
                try
                {
                    return JsonConvert.DeserializeObject<dynamic>(OtherCost);
                }
                catch { }
                return null;
            }
        }
        /// <summary>
        /// 本区招生政策
        /// </summary>
        [JsonIgnore]
        public string AreaRecruitPlan { get; set; }
        [Computed]
        [JsonIgnore]
        public object AreaRecruitPlan_Obj
        {
            get
            {
                try
                {
                    return Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(AreaRecruitPlan);
                }
                catch { }
                return null;
            }
        }
        /// <summary>
        /// 招生计划
        /// </summary>
        public string Plan { get; set; }
        /// <summary>
        /// 招生条件
        /// </summary>
        [JsonIgnore]
        public string Requirement { get; set; }
        [Computed]
        public IEnumerable<string> Requirement_Obj
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(Requirement))
                {
                    try
                    {
                        if (Requirement.Contains("；"))
                        {
                            return JsonConvert.DeserializeObject<IEnumerable<string>>($"[\"{Requirement.Replace("；", "\",\"")}\"]");
                        }
                        else
                        {
                            return JsonConvert.DeserializeObject<IEnumerable<string>>(Requirement);
                        }
                    }
                    catch { }
                }
                return null;
            }
        }
        /// <summary>
        /// 招生班数
        /// </summary>
        public int? ClassQuantity { get; set; }
        /// <summary>
        /// 招生材料
        /// </summary>
        public string Material { get; set; }
        /// <summary>
        /// 招生日程
        /// </summary>
        public string Schedule { get; set; }
        /// <summary>
        /// 学区范围
        /// </summary>
        public string SchoolScope { get; set; }
        /// <summary>
        /// 划片范围(对象)
        /// </summary>
        [JsonIgnore]
        public string ScribingScope { get; set; }
        [Computed]
        public IEnumerable<string[]> ScribingScope_Obj
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(ScribingScope))
                {
                    try
                    {
                        return JsonConvert.DeserializeObject<IEnumerable<string[]>>(ScribingScope);
                    }
                    catch { }
                }
                return null;
            }
        }
        /// <summary>
        /// 划片范围(文字)
        /// </summary>
        public string ScribingScopeStr { get; set; }
        /// <summary>
        /// 统招生招生人数
        /// </summary>
        public int? TZQuantity { get; set; }
        /// <summary>
        /// 调剂生招生人数
        /// </summary>
        public int? TJQuantity { get; set; }
        /// <summary>
        /// 项目班名称
        /// </summary>
        public string ProjectClassName { get; set; }
        /// <summary>
        /// 普招计划人数
        /// </summary>
        public int? PZQuantity { get; set; }
        /// <summary>
        /// 联招计划人数
        /// </summary>
        public int? LZQuantity { get; set; }
        /// <summary>
        /// 中考分数控制线
        /// </summary>
        public int? ZKFSKZX { get; set; }
        /// <summary>
        /// 派位小学EIDs
        /// </summary>
        [JsonIgnore]
        public string AllocationPrimaryEIDs { get; set; }
        [Computed]
        [JsonIgnore]
        public IEnumerable<Guid> AllocationPrimaryEIDs_Obj
        {
            get
            {
                try
                {
                    return JsonConvert.DeserializeObject<IEnumerable<Guid>>(AllocationPrimaryEIDs);
                }
                catch { }
                return null;
            }
        }
        /// <summary>
        /// 对口小学EIDs
        /// </summary>
        [JsonIgnore]
        public string CounterpartPrimaryEIDs { get; set; }
        [Computed]
        [JsonIgnore]
        public IEnumerable<Guid> CounterpartPrimaryEIDs_Obj
        {
            get
            {
                try
                {
                    return JsonConvert.DeserializeObject<IEnumerable<Guid>>(CounterpartPrimaryEIDs);
                }
                catch { }
                return null;
            }
        }
        [JsonIgnore]
        public string ScribingScopeEIDs { get; set; }
        [Computed]
        [JsonIgnore]
        public IEnumerable<Guid> ScribingScopeEIDs_Obj
        {
            get
            {
                try
                {
                    return JsonConvert.DeserializeObject<IEnumerable<Guid>>(ScribingScopeEIDs);
                }
                catch { }
                return null;
            }
        }
    }
}