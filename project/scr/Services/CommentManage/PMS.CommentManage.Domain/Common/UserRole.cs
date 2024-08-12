using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace PMS.CommentsManage.Domain.Common
{
    public enum AdminUserRole : int
    {
        // 1：兼职，2：兼职领队，3：供应商，4：审核，5：管理员

        /// <summary>
        /// 游客
        /// </summary>
        [Description("游客")]
        Visitor = -1,

        /// <summary>
        /// 用户
        /// </summary>
        [Description("用户")]
        Member = 0,

        /// <summary>
        /// 兼职用户
        /// </summary>
        [Description("兼职用户")]
        JobMember = 1,

        /// <summary>
        /// 兼职领队
        /// </summary>
        [Description("兼职领队")]
        JobLeader = 2,

        /// <summary>
        /// 供应商
        /// </summary>
        [Description("供应商")]
        Supplier = 3,

        /// <summary>
        /// 审核
        /// </summary>
        [Description("审核")]
        Assessor = 4,

        /// <summary>
        /// 管理员
        /// </summary>
        [Description("管理员")]
        Admin = 5
    }
}
