using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.OperationPlateform.Domain.Entitys
{
    public class JumpUrl
    {
        public string Id { get; set; }
        
        public string Url { get; set; }
        public string Fw { get; set; }

        public int PV { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
