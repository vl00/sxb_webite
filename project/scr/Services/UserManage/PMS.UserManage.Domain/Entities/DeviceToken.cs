using PMS.UserManage.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.UserManage.Domain.Entities
{
    public class DeviceToken
    {
        public Guid uuID { get; set; }
        public Guid? userID { get; set; }
        public string deviceToken { get; set; }
        public EnumSet.DeviceType type { get; set; }
        public int? city { get; set; }
    }
}
