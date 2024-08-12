using PMS.Search.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Web.ViewModels.Live
{
    public class LiveItemViewModel
    {
        /// <summary>
        /// 点击跳转的链接
        /// </summary>
        public string ActionUrl { get; set; }
        public Guid Id { get; set; }
        public string Title { get; set; }
        public long StartTime { get; set; }
        public int UserCount { get; set; }
        public LectureStatus Status { get; set; }
        public string UserHeadImg { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string CoverHome { get; set; }
        public bool IsCollection { get; set; }
        public int BookCount { get; set; }
    }
}
