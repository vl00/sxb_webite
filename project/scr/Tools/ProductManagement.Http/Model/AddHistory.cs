using System;
using System.Collections.Generic;
using static PMS.UserManage.Domain.Common.EnumSet;

namespace ProductManagement.API.Http.Model
{
    public class AddHistory
    {
        public Guid dataID { get; set; }
        public string iSchoolAuth { get; set; }
        public MessageDataType dataType { get; set; }

        public Dictionary<string, string> cookies { get; set; }
    }
}
