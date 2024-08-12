using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.PCWeb.ViewModels.Article
{
    public class ArticleListItemViewModel
    {
        public Guid Id { get; set; }

        public string No { get; set; }

        public string Title { get; set; }

        public string Time { get; set; }
        /// <summary>
        /// 发表时间(中文)
        /// </summary>
        public string CNTime
        {
            get
            {
                if (DateTime.TryParse(Time, out DateTime convertResult))
                {
                    return convertResult.ToString("yyyy年MM月dd日");
                }
                return Time;
            }
        }

        public int ViewCount { get; set; }

        public int Layout { get; set; }

        public IEnumerable<string> Covers { get; set; }

        public int Type { get; set; }
        public string ShortTitle { get; set; }
    }
}
