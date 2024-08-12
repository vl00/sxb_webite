using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PMS.School.Application.ModelDto
{
    /// <summary>
    /// 学校分数统计
    /// </summary>
    public class SchoolScoreDto
    {
        public Guid SchoolSectionId { get; set; }

        public Guid SchoolId { get; set; }

        /// <summary>
        /// 总分
        /// </summary>
        /// <value>The school score.</value>
        public decimal AggScore { get; set; }

        /// <summary>
        /// 评论数
        /// </summary>
        /// <value>The comment count.</value>
        public int CommentCount { get; set; }


        /// <summary>
        /// 就读评论数
        /// </summary>
        /// <value>The attend count.</value>
        public int AttendCommentCount { get; set; }

        /// <summary>
        /// 提问数
        /// </summary>
        public int QuestionCount { get; set; }

        /// <summary>
        /// 师资力量分
        /// </summary>
        /// <value>The school score.</value>
        public decimal TeachScore { get; set; }
        /// <summary>
        /// 硬件设施分
        /// </summary>
        /// <value>The school score.</value>
        public decimal HardScore { get; set; }
        /// <summary>
        /// 环境周边分
        /// </summary>
        /// <value>The school score.</value>
        public decimal EnvirScore { get; set; }
        /// <summary>
        /// 学风管理分
        /// </summary>
        /// <value>The school score.</value>
        public decimal ManageScore { get; set; }
        /// <summary>
        /// 校园生活分
        /// </summary>
        /// <value>The school score.</value>
        public decimal LifeScore { get; set; }

        /// <summary>
        /// 最后点评时间
        /// </summary>
        /// <value>The last comment time.</value>
        public DateTime LastCommentTime { get; set; }

        /// <summary>
        /// 最后提问时间
        /// </summary>
        /// <value>The last question time.</value>
        public DateTime LastQuestionTime { get; set; }
    }
}
