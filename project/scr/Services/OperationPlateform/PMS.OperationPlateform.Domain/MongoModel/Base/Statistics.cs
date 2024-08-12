using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace PMS.OperationPlateform.Domain.MongoModel.Base
{
    /// <summary>
    /// https://www.bejson.com/convert/json2csharp
    /// </summary>
    public class Statistics
    {
        /// <summary>
        /// 
        /// </summary>
        public int _id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string userid { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string deviceid { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string url { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string method { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? time { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int ip { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int adcode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string fw { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int platform { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int system { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int client { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string version { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string appid { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string _class { get; set; }

        public string @event { get; set; }


        public class StatisticsComparer : IEqualityComparer<Statistics>
        {
            public bool Equals(Statistics x, Statistics y)
            {
                return x.url == y.url;
            }

            public int GetHashCode(Statistics obj)
            {
                return obj.url.GetHashCode();
            }

        }

    }

}
