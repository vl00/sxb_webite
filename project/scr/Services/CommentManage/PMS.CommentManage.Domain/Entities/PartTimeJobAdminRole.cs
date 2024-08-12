using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PMS.CommentsManage.Domain.Entities
{
    [Table("PartTimeJobAdminRoles")]
    public class PartTimeJobAdminRole
    {
        [Key]
        public int Id { get; set; }

        public Guid AdminId { get; set; }

        public int Role { get; set; }

        public Guid? ParentId { get; set; }

        public bool Shield { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreateTime { get; set; }

        [ForeignKey("AdminId")]
        public virtual PartTimeJobAdmin PartTimeJobAdmin { get; set; }
    }
}
