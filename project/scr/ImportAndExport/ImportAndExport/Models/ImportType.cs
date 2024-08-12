using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace ImportAndExport.Models
{
    public enum ImportType : int
    {
        [Description("达人邀请写入")]
        Daren = 1,

        [Description("ugc激励方案")]
        UgcExcitation = 2,

        [Description("ugc数据抓取导入")]
        Ugc = 3
    }
}
