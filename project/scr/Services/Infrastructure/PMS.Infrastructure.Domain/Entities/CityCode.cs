using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.Infrastructure.Domain
{
    /// <summary>
    /// 城市列表
    /// </summary>
    public class CityCode
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int Parent { get; set; }

    }
}
