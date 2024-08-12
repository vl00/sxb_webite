using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMS.UserManage.Domain.Common
{
    public class RootModel
    {
        public int status { get; set; }
        public string errorDescription { get; set; }
        public string ReturnUrl { get; set; }
        public object data { get; set; }
    }
}
