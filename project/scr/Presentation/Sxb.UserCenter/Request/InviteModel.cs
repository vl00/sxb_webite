using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.UserCenter.Request
{
    public class InviteModel
    {
        public List<Guid> Ids { get; set; }
         public   bool IsAll { get; set; }
    }
}
