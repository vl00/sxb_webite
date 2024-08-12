using System;
namespace PMS.CommentsManage.Application.ModelDto
{
    public class SchoolCommentScoreDto
    {
        public Guid SchoolId { get; set; }
        public Guid SchoolSectionId { get; set; }

        public Guid UserId { get; set; }

        public string SchoolName { get; set; }
       
        public DateTime UpdateTime { get; set; }

        public bool IsAttend { get; set; }

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
        /// 提问数
        /// </summary>
        public int QuestionCount { get; set; }
    }
}
