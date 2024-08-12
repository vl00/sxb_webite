using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.Infrastructure.Application.ModelDto
{
    /// <summary>
    /// 城市列表
    /// </summary>
    public class CityCodeDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        /// <summary>
        /// 父id
        /// </summary>
        public int Parent { get; set; }
    }
}
