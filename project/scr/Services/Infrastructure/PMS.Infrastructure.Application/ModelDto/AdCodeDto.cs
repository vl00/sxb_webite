using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMS.Infrastructure.Application.ModelDto
{
    public class AdCodeDto
    {
        public string Key { get; set; }
        public List<CityCodeDto> CityCodes { get; set; }
    }

    /// <summary>
    /// 获得省 城市的code
    /// </summary>
    public class ProvinceCodeDto
    {
        public string Province { get; set; }

        public int ProvinceId { get; set; }

        public List<CityCodeDto> CityCodes { get; set; }
    }



}
