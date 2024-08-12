using PMS.OperationPlateform.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Web.Areas.Advertising.Models
{
    public class AdvGetAdvsResponse
    {
        public int status { get; set; }

        public string msg { get; set; }

        public List<AdvOption> Advs { get; set; }

        public class AdvOption
        {
            public int Place { get; set; }

            public List<AdvertisingBaseGetAdvertisingResultDto> Items { get; set; }

            public IEnumerable<dynamic> Courses { get; set; } 
        }
    }
}
