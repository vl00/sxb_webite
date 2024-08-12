using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PMS.CommentsManage.Domain.Entities
{
    [Table("SchoolTags")]
    public class SchoolTag
    {
        public SchoolTag()
        {
            Id = Guid.NewGuid();
        }

        [Key]
        public Guid Id { get; set; }
        [ForeignKey("Tags")]
        public Guid TagId { get; set; }
        public Guid SchoolId { get; set; }
        [ForeignKey("UserInfo")]
        public Guid UserId { get; set; }
        [ForeignKey("SchoolComments")]
        public Guid SchoolCommentId { get; set; }
        public virtual SchoolComment SchoolComment { get; set; }
        public virtual CommentTag Tag { get; set; }
    }
}
