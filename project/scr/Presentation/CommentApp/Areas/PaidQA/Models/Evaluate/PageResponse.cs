using Newtonsoft.Json;
using ProductManagement.Framework.Foundation;
using Sxb.Web.Utils;
using System;
using System.Collections.Generic;
using PMS.PaidQA.Domain.EntityExtend;

namespace Sxb.Web.Areas.PaidQA.Models.Evaluate
{
    public class PageResponse
    {
        public int Total { get; set; }
        public int AvgScore { get; set; }
        public IEnumerable<EvaluateItem> Evaluates { get; set; }
        public IEnumerable<EvaluateTagCountingExtend> Tags { get; set; }
    }
    public class EvaluateItem
    {
        public string HeadImgUrl { get; set; }
        public string NickName { get; set; }
        public int Score { get; set; }
        public Guid OrderID { get; set; }
        public string Content { get; set; }
        [JsonConverter(typeof(HmDateTimeConverter))]
        public DateTime CreateTime { get; set; }
        public string FormatCreateTime
        {
            get
            {
                return CreateTime.ConciseTime("yyyy年MM月dd日");
            }
        }
    }
}
