using Dapper.Contrib.Extensions;
using Newtonsoft.Json;
using System;

namespace PMS.School.Domain.Entities.WechatDemo
{
    /// <summary>
    /// 学部其他信息
    /// </summary>
    [Serializable]
    [Table(nameof(SchoolOverViewInfo))]
    public class SchoolOverViewInfo
    {
        [JsonIgnore]
        [ExplicitKey]
        public Guid ID { get; set; }
        public Guid EID { get; set; }
        public Guid? SID { get; set; }
        /// <summary>
        /// 是否有校车
        /// </summary>
        public bool HasSchoolBus { get; set; }
        /// <summary>
        /// 招生方式
        /// </summary>
        [JsonIgnore]
        public string RecruitWay { get; set; }
        [Computed]
        public object RecruitWay_Obj
        {
            get
            {
                try
                {
                    return JsonConvert.DeserializeObject<dynamic>(RecruitWay);
                }
                catch { }
                return null;
            }
        }
        /// <summary>
        /// 学校公众号名称
        /// </summary>
        public string OAName { get; set; }
        /// <summary>
        /// 学校公众号appid
        /// </summary>
        public string OAAppID { get; set; }
        /// <summary>
        /// 学校小程序名称
        /// </summary>
        public string MPName { get; set; }
        /// <summary>
        /// 学校小程序appid
        /// </summary>
        public string MPAppID { get; set; }
        /// <summary>
        /// 学校视频号名称
        /// </summary>
        public string VANAme { get; set; }
        /// <summary>
        /// 学校视频号appid
        /// </summary>
        public string VAAppID { get; set; }
        /// <summary>
        /// 学校公众号帐号
        /// </summary>
        public string OAAccount { get; set; }
        /// <summary>
        /// 小程序帐号
        /// </summary>
        public string MPAccount { get; set; }
        /// <summary>
        /// 视频号帐号
        /// </summary>
        public string VAAccount { get; set; }
        /// <summary>
        /// 学部获得的认证
        /// </summary>
        [JsonIgnore]
        public string Certifications { get; set; }
        [Computed]
        public object Certifications_Obj
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(Certifications))
                {
                    try
                    {
                        return JsonConvert.DeserializeObject<dynamic>(Certifications);
                    }
                    catch { }
                }
                return null;
            }
        }
    }
}