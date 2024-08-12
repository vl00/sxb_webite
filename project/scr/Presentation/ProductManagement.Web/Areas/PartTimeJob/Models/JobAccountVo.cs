using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductManagement.Web.Areas.PartTimeJob.Models
{
    /// <summary>
    /// 兼职人员账号信息
    /// </summary>
    public class JobAccountVo
    {
        public Guid Id { get; set; }
        /// <summary>
        /// 兼职昵称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 兼职手机号
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// 所属供应商
        /// </summary>
        public string Supplier { get; set; }
        /// <summary>
        /// 所属领队
        /// </summary>
        public string Leader { get; set; }
        /// <summary>
        /// 邀请码
        /// </summary>
        public string InvitationCode { get; set; }
        /// <summary>
        /// 注册时间
        /// </summary>
        public string RegisterTime { get; set; }
        /// <summary>
        /// 是否被拉黑
        /// </summary>
        public bool Shield { get; set; }
    }
}
