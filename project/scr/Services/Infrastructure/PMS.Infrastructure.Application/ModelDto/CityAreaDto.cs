using System;
using System.Collections.Generic;

namespace PMS.Infrastructure.Application.ModelDto
{
    public class CityAreaDto
    {
        public int CityCode { get; set; }

        public List<AreaDto> Areas { get; set; }
    }
}
