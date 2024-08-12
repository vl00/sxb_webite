using Newtonsoft.Json;
using ProductManagement.Framework.AspNetCoreHelper.Utils;
using ProductManagement.Framework.Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Web.Areas.PaidQA.Models
{
    public class SearchRenderArticleResult
    {

        /// <summary>
        /// 标识
        /// </summary>
        public Guid Id { get; set; }

        public string No { get; set; }

        /// <summary>
        /// 文章标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 发表时间
        /// </summary>
        public string Time { get; set; }

        /// <summary>
        /// 该字段存真正的Time，由于Time被上面的格式化Time占用了，所以扩展_Time字段。
        /// </summary>
        public DateTime _Time { get; set; }
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 阅读量
        /// </summary>
        public int ViewCount { get; set; }

        public int Layout { get; set; }

        public IEnumerable<string> Covers { get; set; }

        public int Type { get; set; }

        public string Html { get; set; }
        public string ShortTitle { get; set; }

    }
}
