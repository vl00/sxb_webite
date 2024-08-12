using Dapper.Contrib.Extensions;
using Newtonsoft.Json;
using System;

namespace PMS.School.Domain.Entities.WechatDemo
{
    /// <summary>
    /// 开设的项目与课程
    /// </summary>
    [Serializable]
    [Table(nameof(OnlineSchoolProjectInfo))]
    public class OnlineSchoolProjectInfo
    {
        [JsonIgnore]
        [ExplicitKey]
        public Guid ID { get; set; }
        [JsonIgnore]
        public Guid EID { get; set; }
        public string Name { get; set; }
        [JsonIgnore]
        public string ItemJson { get; set; }
        [Computed]
        public object Item_Obj
        {
            get
            {
                try
                {
                    return JsonConvert.DeserializeObject<dynamic>(ItemJson);
                }
                catch { }
                return null;
            }
        }
        public bool IsDeleted { get; set; }
    }
}