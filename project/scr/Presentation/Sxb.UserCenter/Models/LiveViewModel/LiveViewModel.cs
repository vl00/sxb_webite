using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.UserCenter.Models.LiveViewModel
{
    /// <summary>
    /// 微课视图实体
    /// </summary>
    public class LiveViewModel
    {
        /// <summary>
        /// 直播id
        /// </summary>
        public Guid Id { get; set; }

        public string Title { get; set; }

        public Guid LectorId { get; set; }

        public string LectorName { get; set; }

        public string LectorHeadImg { get; set; }

        /// <summary>
        /// 直播日期时间
        /// </summary>
        public string Time { get; set; }
        /// <summary>
        /// 封面图
        /// </summary>
        public string FrontCover { get; set; }
        /// <summary>
        /// 收听人数
        /// </summary>
        public int ViewCount { get; set; }
    }
}
