using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.PaidQA.Domain.MongoModel
{
    [BsonIgnoreExtraElements]
    public class statistics
    {
        /// <summary>
        /// 渠道
        /// </summary>
        public string fw { get; set; }
        public DateTime time { get; set; }
        public string url { get; set; }
    }
}
