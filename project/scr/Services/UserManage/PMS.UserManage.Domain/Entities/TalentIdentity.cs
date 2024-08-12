using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PMS.UserManage.Domain.Entities
{
    [Table("talent_identity")]
    public class TalentIdentity
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Key]
        public Guid id { get; set; }
        /// <summary>
        /// 达人身份名称
        /// </summary>

        public string identity_name { get; set; }
        /// <summary>
        /// 类型 0个人 1机构
        /// </summary>
        public int? type { get; set; }
        /// <summary>
        /// 是否启用 0为否 1为是
        /// </summary>
        public int? enable { get; set; }
        /// <summary>
        /// 达人身份描述
        /// </summary>
        public string identity_description { get; set; }
        /// <summary>
        /// 删除标记 0否，1是
        /// </summary>
        public int isdelete { get; set; } = 0;
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? createdate { get; set; }
    }
}
