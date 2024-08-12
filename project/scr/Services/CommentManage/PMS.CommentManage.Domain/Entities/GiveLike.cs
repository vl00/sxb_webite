using PMS.CommentsManage.Domain.Common;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PMS.CommentsManage.Domain.Entities
{   
    /// <summary>
    /// 点赞
    /// </summary>
    public class GiveLike
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public LikeType LikeType { get; set; }
        /// <summary>
        /// 点评 / 问题 / 回复 id
        /// </summary>
        [Required]
        public Guid SourceId { get; set; }
        public Guid ReplyId { get; set; }
        [Required]
        public Guid UserId { get; set; }

        /// <summary>
        /// 渠道标识
        /// </summary>
        public string Channel { get; set; }

        /// <summary>
        /// 在添加实体时仓储自动分析
        /// </summary>
        public LikeStatus LikeStatus { get; set; }

        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreateTime { get; set; }
    }
}
