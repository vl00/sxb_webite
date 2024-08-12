using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PMS.CommentsManage.Domain.Entities
{
    public class SchoolCommentReportReply
    {
        [Key]
        public Guid Id { get; set; }

        public Guid ReportId { get; set; }

        public Guid AdminId { get; set; }

        public string ReplyContent { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreateTime { get; set; }

        [ForeignKey("ReportId")]
        public virtual SchoolCommentReport SchoolCommentReport { get; set; }
    }
}
