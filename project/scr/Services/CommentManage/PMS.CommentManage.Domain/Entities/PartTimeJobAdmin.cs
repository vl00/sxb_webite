using PMS.CommentsManage.Domain.Common;
using ProductManagement.Framework.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PMS.CommentsManage.Domain.Entities
{
    /// <summary>
    /// 兼职系统人员信息， 兼职人员无法进行登录系统
    /// </summary>
    [Table("PartTimeJobAdmins")]
    public class PartTimeJobAdmin
    {
        [Key]
        public Guid Id { get; set; }
        /// <summary>
        /// 电话（账号）
        /// </summary>
        [MaxLength(11)]
        public string Phone { get; set; }
        /// <summary>
        /// 系统内昵称（允许修改）
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 1：兼职，2：兼职领队，3：供应商，4：审核，5：管理员            
        /// 由于第二次需求变更，用户可存在多个角色。用户写点评时，首先查询该表判断该用户是否为兼职用户，如果为兼职用户则点评内容中会标识为兼职用户发表
        /// 所以现该表该字段只储存 管理员、审核、供应商、兼职 身份
        /// 其余身份信息存在PartTimeJobAdminRole表中
        /// </summary>
        [Required]
        public AdminUserRole Role { get; set; }
        /// <summary>
        /// 第一次登录系统密码为邀请码，则password为空，更改密码后存储MD5加密后的结果
        /// </summary>
        [MaxLength(32)]
        public string Password { get; set; }
        /// <summary>
        /// 父级Id
        /// </summary>
        [Required]
        public Guid ParentId { get; set; }
        /// <summary>
        /// 邀请码（用来邀请其他人入驻系统）
        /// </summary>
        [MaxLength(8)]
        public string InvitationCode { get; set; }
        /// <summary>
        /// 是否禁用该账号，ture：禁用
        /// </summary>
        public bool Prohibit { get; set; }
        /// <summary>
        /// 结算类型，0：为子级类型结算方式以供应商的主，1：微信现结，2：合同另结
        /// </summary>
        public SettlementType SettlementType { get; set; }
        /// <summary>
        /// 注册日期，激活日期（查询日志表）
        /// </summary>
        [Required]
        public DateTime RegesitTime { get; set; }
        /// <summary>
        /// 结算详情
        /// </summary>
        public virtual List<SettlementAmountMoney> SettlementAmountMoneys { get; set; }

        public virtual List<PartTimeJobAdminRole> PartTimeJobAdminRoles { get; set; }
    }
}
