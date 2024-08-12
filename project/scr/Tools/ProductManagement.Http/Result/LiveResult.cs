using System;
using System.Collections.Generic;
using System.Text;

namespace ProductManagement.API.Http.Result
{
    /// <summary>
    /// 直播api实体
    /// </summary>
    public class LiveResult
    {
        /// <summary>
        /// 直播id
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 封面图
        /// </summary>
        public string FrontCover { get; set; }
        /// <summary>
        /// 观看人数
        /// </summary>
        public int ViewCount { get; set; }
        /// <summary>
        /// 添加时间
        /// </summary>
        public DateTime AddTime { get; set; }
    }
}
