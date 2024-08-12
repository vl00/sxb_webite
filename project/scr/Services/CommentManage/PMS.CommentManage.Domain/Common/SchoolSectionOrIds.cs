using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace PMS.CommentsManage.Domain.Common
{
    public enum SchoolSectionOrIds
    {
        [Description("点评Id集合")]
        IdS = 1,

        [Description("学校分部Id集合")]
        SchoolSectionIds = 2
    }
}
