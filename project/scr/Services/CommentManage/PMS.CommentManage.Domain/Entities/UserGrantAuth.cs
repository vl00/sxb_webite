using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PMS.CommentsManage.Domain.Entities
{
    /// <summary>
    /// 用户授权
    /// </summary>
    [Table("UserGrantAuths")]
    public class UserGrantAuth
    {
        [Key]
        public int Id { get; set; }
        public Guid UserId { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime GrantAuthTime { get; set; }
    }
}
