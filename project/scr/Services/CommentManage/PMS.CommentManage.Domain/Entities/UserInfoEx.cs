using PMS.CommentsManage.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PMS.CommentsManage.Domain.Entities
{
    [Table("UserInfo")]
    public class UserInfoEx
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string NickName { get; set; }

        [Required]
        public string Phone { get; set; }

        [Required]
        public PMS.UserManage.Domain.Common.UserRole Role { get; set; }

        public string HeadImager { get; set; }

        public virtual List<QuestionInfo> QuestionInfo { get; set; }
        public virtual List<QuestionsAnswersInfo> QuestionsAnswersInfos { get; set; }
        public virtual List<QuestionsAnswersReport> QuestionsAnswersReports { get; set; }

        public virtual List<SchoolComment> SchoolComment { get; set; }
        public virtual List<SchoolCommentReport> SchoolCommentReport { get; set; }
    }
}
