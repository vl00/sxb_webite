using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PMS.CommentsManage.Domain.Entities
{
    [Table("Tags")]
    public class CommentTag
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
    }
}
