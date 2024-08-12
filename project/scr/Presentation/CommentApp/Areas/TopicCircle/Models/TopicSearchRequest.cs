using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static PMS.TopicCircle.Domain.Enum.GlobalEnum;

namespace Sxb.Web.Areas.TopicCircle.Models
{
    public class TopicSearchRequest
    {
        public string Keyword { get; set; }
        public Guid? CircleId { get; set; }
        public Guid? UserId { get; set; }
        public int? Type { get; set; }
        public TopicTopType? TopType { get; set; }
        /// <summary>
        /// 精品话题
        /// </summary>
        public bool? IsGood { get; set; }
        /// <summary>
        /// 问答话题
        /// </summary>
        public bool? IsQA { get; set; }
        /// <summary>
        /// 圈主的话题
        /// </summary>
        public bool? IsCircleOwner { get; set; }

        public List<string> Tags { get; set; }
        //public List<string> TagIds { get; set; }

        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }
}
