using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace PMS.UserManage.Domain.Common
{
    public enum UserRole : int
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
        /// 个人达人
        /// </summary>
        [Description("个人达人")]
        PersonTalent = 1,
        /// <summary>
        /// 机构达人
        /// </summary>
        [Description("机构达人")]
        OrgTalent = 2,

        /// <summary>
        /// 校方
        /// </summary>
        [Description("校方")]
        School = 3,


        /// <summary>
        /// 专家
        /// </summary>
        [Description("专家")]
        Recommend = 4,


        /// <summary>
        /// 兼职用户
        /// </summary>
        [Description("兼职用户")]
        JobMember = 99
    }
}
