using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PMS.CommentsManage.Domain.Entities
{
    /// <summary>
    /// 点评审核
    /// </summary>
    [Table("SchoolCommentExamines")]
    public class SchoolCommentExamine
    {
        [Key]
        public Guid Id { get; set; }
        /// <summary>
        /// 审核者
        /// </summary>
        public Guid AdminId { get; set; }

        public bool IsPartTimeJob { get; set; }

        /// <summary>
        /// 点评
        /// </summary>
        public Guid SchoolCommentId { get; set; }
        /// <summary>
        /// 审核日期
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime? AddTime { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime? UpdateTime { get; set; }

        [ForeignKey("SchoolCommentId")]
        public virtual SchoolComment SchoolComment { get; set; }
    }
}
