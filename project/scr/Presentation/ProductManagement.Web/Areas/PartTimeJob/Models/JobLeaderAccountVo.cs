using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductManagement.Web.Areas.PartTimeJob.Models
{
    /// <summary>
    /// 兼职领队账号信息
    /// </summary>
    public class JobLeaderAccountVo
    {
        public Guid Id { get; set; }
        /// <summary>
        /// 兼职领队昵称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 联系方式
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// 所属供应商
        /// </summary>
        public string Supplier { get; set; }
        /// <summary>
        /// 邀请码
        /// </summary>
        public string InvitationCode { get; set; }
        /// <summary>
        /// 账号注册时间
        /// </summary>
        public string RegisterTime { get; set; }
        /// <summary>
        /// 是否禁用该账号，ture：禁用
        /// </summary>
        public bool Prohibit { get; set; }
    }
}
