using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMS.UserManage.Domain.Common
{
    public class KeyValue
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }

    public class InterestItem : KeyValue
    {
        public bool selected { get; set; }
    }
}
