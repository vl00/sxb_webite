using System;
namespace PMS.CommentsManage.Application.ModelDto
{
    public class SchoolScoreDto
    {
        public Guid SchoolId { get; set; }
        public Guid SchoolSectionId { get; set; }

        /// <summary>
        /// 总分
        /// </summary>
        public decimal AggScore { get; set; }

        /// <summary>
        /// 师资力量分
        /// </summary>
        public decimal TeachScore { get; set; }

        /// <summary>
        /// 硬件设施分
        /// </summary>
        public decimal HardScore { get; set; }

        /// <summary>
        /// 环境周边分
        /// </summary>
        public decimal EnvirScore { get; set; }

        /// <summary>
        /// 学风管理分
        /// </summary>
        public decimal ManageScore { get; set; }

        /// <summary>
        /// 校园生活分
        /// </summary>
        public decimal LifeScore { get; set; }

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
        /// 最后点评时间
        /// </summary>
        /// <value>The last comment time.</value>
        public DateTime LastCommentTime { get; set; }

        /// <summary>
        /// 提问数
        /// </summary>
        /// <value>The Question count.</value>
        public int QuestionCount { get; set; }

        /// <summary>
        /// 最后点评时间
        /// </summary>
        /// <value>The last Question time.</value>
        public DateTime LastQuestionTime { get; set; }
    }
}
