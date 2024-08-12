using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMS.Infrastructure.Application.ModelDto
{
    /// <summary>
    /// 当前城市的区域信息
    /// </summary>
    public class AreaDto
    {
        /// <summary>
        /// 地区AreaCode
        /// </summary>
        public int AdCode { get; set; }
        /// <summary>
        /// 地区名称
        /// </summary>
        public string AreaName { get; set; }
        public bool active { get; set; } = false;
    }
}
