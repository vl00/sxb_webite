using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMS.UserManage.Application.ModelDto.Account
{
    public class DestroyCondition
    {
        public bool SafeStatus { get; set; }
        public bool ServiceComplete { get; set; }
        public bool NoPublish { get; set; }
        public bool NoPunish { get; set; }
    }
}
