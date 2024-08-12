using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductManagement.Framework.MSSQLAccessor.WebExample.Core
{
    public class GroupOrderProgressDomain
    {
        public string grouporder_id { get; set; }
        public string user_id { get; set; }
        public int hm_go_paystatus { get; set; }
    }
}
