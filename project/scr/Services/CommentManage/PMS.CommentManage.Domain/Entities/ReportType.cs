using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PMS.CommentsManage.Domain.Entities
{
    [Table("ReportTypes")]
    public class ReportType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string TypeName { get; set; }

        public int Sort { get; set; }
    }
}
