using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.UserManage.Application.ModelDto
{
    public class VoBase
    {
        public bool success { get; set; }
        public int status { get; set; }
        public object data { get; set; }
    }
}
