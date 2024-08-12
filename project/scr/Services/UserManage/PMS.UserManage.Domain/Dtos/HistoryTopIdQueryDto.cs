using System;
using System.Collections.Generic;
using System.Text;
using static PMS.UserManage.Domain.Common.EnumSet;

namespace PMS.UserManage.Domain.Dtos
{
    public class HistoryTopIdQueryDto
    {
        /// <summary>
        /// 对象Id 
        /// </summary>
        public Guid DataId { get; set; }

        /// <summary>
        /// 浏览次数
        /// </summary>
        public long ViewCount { get; set; }

        /// <summary>
        /// 最后浏览时间
        /// </summary>
        public DateTime LastViewTime{ get; set; }
    }
}
