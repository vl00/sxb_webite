using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductManagement.Framework.MSSQLAccessor.WebExample.Core
{
    public class OrderStatusLogDomain
    {
        /// <summary>
        /// id
        /// </summary>
        public int hm_reflog_id { get; set; }

        /// <summary>
        /// 订单号
        /// </summary>
        public string hm_orderid { get; set; }

        /// <summary>
        /// 订单类型
        /// </summary>
        public int hm_ordertype { get; set; }

        /// <summary>
        /// 订单状态
        /// </summary>
        public int hm_orderstatus { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime hm_createtime { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string hm_remark { get; set; }
    }
}
