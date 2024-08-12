using Newtonsoft.Json;
using PMS.School.Domain.Common;
using ProductManagement.Framework.Foundation;
using System;

namespace PMS.School.Domain.Dtos
{
    public class SchExtDto1
    {
        /// <summary>学部id</summary>
        public Guid Eid { get; set; }
        /// <summary>学部短id</summary>
        public string Eid_s => UrlShortIdUtil.Long2Base32(_Eno);
        /// <summary>学校id</summary>
        public Guid Sid { get; set; }
        /// <summary>学校名</summary>
        public string SchName { get; set; }
        /// <summary>学部名</summary>
        public string ExtName { get; set; }
        /// <summary>纬度</summary> 
        public double? Latitude { get; set; }
        /// <summary>经度</summary> 
        public double? Longitude { get; set; }

        [JsonIgnore]
        public long _Eno { get; set; }
    }
}
