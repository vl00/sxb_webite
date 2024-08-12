using PMS.Live.Domain.Enums;
using System;

namespace PMS.Live.Domain.Dtos
{
    public class LectureDetailDto
    {
        /// <summary>
        /// 直播ID
        /// </summary>
        public Guid ID { get; set; }
        /// <summary>
        /// 直播状态
        /// </summary>
        public LectureStatus Status { get; set; }
        /// <summary>
        /// 直播封面
        /// </summary>
        public string CoverUrl { get; set; }
        /// <summary>
        /// 浏览次数
        /// </summary>
        public long ViewCount { get; set; }
        /// <summary>
        /// 讲师ID
        /// </summary>
        public Guid LectorID { get; set; }
        /// <summary>
        /// 添加时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 直播标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 讲师用户ID
        /// </summary>
        public Guid LectorUserID { get; set; }
    }
}