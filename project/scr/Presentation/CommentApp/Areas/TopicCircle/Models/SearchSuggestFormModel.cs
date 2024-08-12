using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static PMS.Search.Domain.Common.GlobalEnum;

namespace Sxb.Web.Areas.TopicCircle.Models
{
    public class SearchSuggestFormModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string KeyWords { get; set; }
        /// <summary>
        /// 1 学校 16 文章  32 直播
        /// </summary>
        public ChannelIndex? Channel { get; set; }
        /// <summary>
        /// 是否查询达人相关的数据
        /// </summary>
        public bool IsRelated { get; set; }



        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
