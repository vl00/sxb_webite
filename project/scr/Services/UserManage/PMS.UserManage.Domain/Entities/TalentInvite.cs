using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PMS.UserManage.Domain.Entities
{
    [Table("talent_invite")]
    public class TalentInvite
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Key]
        public Guid id { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? createdate { get; set; }

        /// <summary>
        /// 有效时间
        /// </summary>
        public DateTime? effectivedate { get; set; }

        /// <summary>
        /// 邀请码
        /// </summary>
        public string code { get; set; }

        /// <summary>
        /// 邀请表主键
        /// </summary>
        public Guid parent_id { get; set; }

        /// <summary>
        /// 邀请类型 0:达人，1机构员工
        /// </summary>
        public int? type { get; set; }

        /// <summary>
        /// 状态 0 未使用  1 已使用
        /// </summary>
        public int? status { get; set; }

    }
}
