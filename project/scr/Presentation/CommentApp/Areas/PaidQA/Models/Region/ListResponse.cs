using System;
using System.Collections.Generic;

namespace Sxb.Web.Areas.PaidQA.Models.Region
{
    public class ListResponse : ResponseItem
    {
        public IEnumerable<ResponseItem> SubItems { get; set; }
    }

    public class ResponseItem
    {
        /// <summary>
        /// 领域ID
        /// </summary>
        public Guid ID { get; set; }
        /// <summary>
        /// 领域名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }
    }
}
