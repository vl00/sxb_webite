using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductManagement.Web.Areas.PartTimeJob.Models.ViewEntity
{
    public class ProcGetAdminByRoleTypeEntityVo
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
        public string SettlementType { get; set; }
        public string InvitationCode { get; set; }
    }
}
