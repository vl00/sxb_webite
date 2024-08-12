using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Web.RequestModel
{
    public class PageRequest
    {
        /// <summary>
        /// 偏移量
        /// </summary>
        public int Offset { get; set; }

        /// <summary>
        /// 限制条数
        /// </summary>
        public int Limit { get; set; } = 20;

    }
}
