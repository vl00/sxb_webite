using System;

namespace PMS.PaidQA.Domain.Dtos
{
    public class PageTalentIDDto
    {
        /// <summary>
        /// 达人用户ID
        /// </summary>
        public Guid TalentUserID { get; set; }
        /// <summary>
        /// 价格
        /// </summary>
        public decimal Price { get; set; }
        /// <summary>
        /// 平均回复间隔
        /// </summary>
        public int AvgReplyTimespan { get; set; }
        /// <summary>
        /// 等级排序
        /// </summary>
        public int LevelSort { get; set; }
        /// <summary>
        /// 平均分
        /// </summary>
        public int AvgScore { get; set; }
        /// <summary>
        /// 6小时回复率
        /// </summary>
        public int ReplyPercent { get; set; }
    }
}
