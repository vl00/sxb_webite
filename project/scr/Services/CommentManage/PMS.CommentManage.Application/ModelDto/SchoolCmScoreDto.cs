using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.CommentsManage.Application.ModelDto
{
    public class SchoolCmScoreDto
    {
        /// <summary>
        /// 点评Id
        /// </summary>
        public Guid CommentId { get; set; }
        /// <summary>
        /// 是否就读
        /// </summary>
        public bool IsAttend { get; set; }

        /// <summary>
        /// 总星（总分数）
        /// </summary>
        public int AggStar { get; set; }
        /// <summary>
        /// 师资力量分
        /// </summary>
        public int TeachStar { get; set; }
        /// <summary>
        /// 硬件设施分
        /// </summary>
        public int HardStar { get; set; }
        /// <summary>
        /// 环境周边分
        /// </summary>
        public int EnvirStar { get; set; }
        /// <summary>
        /// 学风管理分
        /// </summary>
        public int ManageStar { get; set; }
        /// <summary>
        /// 校园生活分
        /// </summary>
        public int LifeStar { get; set; }
    }
}
