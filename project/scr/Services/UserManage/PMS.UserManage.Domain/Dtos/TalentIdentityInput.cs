using System;

namespace PMS.UserManage.Domain.Dtos
{
    public class TalentIdentityInput
    {
        /// <summary>
        /// 主键
        /// </summary>
        public Guid? id { get; set; }
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
    }
}
