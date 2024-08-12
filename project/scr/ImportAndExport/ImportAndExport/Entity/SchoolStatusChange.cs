using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace ImportAndExport.Entity
{
    public class SchoolStatusChange
    {
        [Description("用户手机（脱敏）")]
        public string Phone { get; set; }

        [Description("点评内容")]
        public string Content { get; set; }
    }
}
