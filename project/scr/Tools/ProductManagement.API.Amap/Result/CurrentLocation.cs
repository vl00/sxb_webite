using System;
using System.Collections.Generic;
using System.Text;

namespace ProductManagement.Tool.Amap.Result
{
    public class CurrentLocation
    {
        public int status { get; set; }
        public string info { get; set; }
        public int infocode { get; set; }
        public string province { get; set; }
        public string city { get; set; }
        public int adcode { get; set; }
        public string rectangle { get; set; }
    }
}
