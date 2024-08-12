using System;
using System.Collections.Generic;
using System.Text;

namespace ProductManagement.API.SMS.Model
{
    public class RndCodeModel
    {
        public string Mobile { get; set; }
        public string Code { get; set; }
        public string CodeType { get; set; }
        public DateTime CodeTime { get; set; }
    }
}
