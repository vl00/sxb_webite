using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PMS.UserManage.Domain.Entities
{
    [Table("talent_audit")]
    public class TalentAudit
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Key]
        public Guid id { get; set; }

        /// <summary>
        /// 达人表主键
        /// </summary>
        public Guid talent_id { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? createdate { get; set; }

        /// <summary>
        /// 状态 0 不通过，1通过
        /// </summary>
        public int? status { get; set; }

        /// <summary>
        /// 审核时间
        /// </summary>
        public DateTime? audit_date { get; set; }

        /// <summary>
        /// 其他原因
        /// </summary>
        public string audit_reason { get; set; }

        /// <summary>
        /// 审核人(预留)
        /// </summary>
        public Guid? audit_userid { get; set; }

        /// <summary>
        /// 驳回原因 0材料不足，1不符合认证身份，2其他
        /// </summary>
        public int? rejection_type { get; set; }

        /// <summary>
        /// 认证类型 0认证称号，1认证称号+后缀
        /// </summary>
        public int? authentication_type { get; set; }
    }
}
