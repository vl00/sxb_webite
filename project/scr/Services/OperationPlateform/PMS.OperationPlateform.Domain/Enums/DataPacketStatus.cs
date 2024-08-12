using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.OperationPlateform.Domain.Enums
{
    /// <summary>
    /// 状态  1 扫码  2 关注
    /// </summary>
    public enum DataPacketStatus
    {
        /// <summary>
        /// 无
        /// </summary>
        None = 0,
        /// <summary>
        /// 扫码
        /// </summary>
        Sacn = 1,
        /// <summary>
        /// 关注
        /// </summary>
        Subscribe = 2
    }
}
