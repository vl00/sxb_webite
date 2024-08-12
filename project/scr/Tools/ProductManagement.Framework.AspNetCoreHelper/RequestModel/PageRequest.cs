using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductManagement.Framework.AspNetCoreHelper.RequestModel
{
    public class PageRequest
    {
        public int PageIndex { get; set; } = 1;

        public int PageSize { get; set; } = 20;

    }
}
