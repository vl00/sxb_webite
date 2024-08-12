using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.CommentsManage.Domain.Entities.ProcViewEntities
{
    /// <summary>
    /// 根据角色类型获取该子集下的所有实体（存储过程：proc_GetAdminByRoleType 参数：@AdminByRoleTypeId(父级类型)，@OutRoleType(子级类型)）
    /// </summary>
    public class ProcGetAdminByRoleTypeEntity
    {
        public Guid Id { get; set; }
        /// <summary>
        /// 当前父级下子元素的个数
        /// </summary>
        public int Total { get; set; }
        public string Phone { get; set; }
        public string Name { get; set; }
        /// <summary>
        /// 结算类型
        /// </summary>
        public int SettlementType { get; set; }
        /// <summary>
        /// 邀请码
        /// </summary>
        public string InvitationCode { get; set; }
    }
}
