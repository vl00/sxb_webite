using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PMS.CommentsManage.Domain.Entities
{
    public class SchoolCommentScore
    {
        public SchoolCommentScore()
        {
            Id = Guid.NewGuid();

        }
        [Key]
        public Guid Id { get; set; }
        /// <summary>
        /// 点评ID
        /// </summary>
        /// <value>The report user identifier.</value>
        [Required]
        public Guid CommentId { get; set; }
        /// <summary>
        /// 是否就读
        /// </summary>
        [Required]
        public bool IsAttend { get; set; }
        /// <summary>
        /// 总分
        /// </summary>
        /// <value>The school score.</value>
        public decimal AggScore { get; set; }
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
        /// 写入日期
        /// </summary>
        //[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        //public DateTime AddTime { get; set; }

        [ForeignKey("CommentId")]
        public virtual SchoolComment SchoolComment { get; set; }
    }
}
