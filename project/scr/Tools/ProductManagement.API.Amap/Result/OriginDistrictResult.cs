using System;
using System.Collections.Generic;

namespace ProductManagement.Tool.Amap.Result
{
    public class OriginDistrictResult
    {
        public string Status { get; set; }
        public string Info { get; set; }
        public string Infocode { get; set; }
        public string Count { get; set; }

        public object Suggestion { get; set; }

        public List<DistrictData> Districts { get; set; }

        public class DistrictData
        {
            public object Citycode { get; set; }
            public string Adcode { get; set; }
            public string Name { get; set; }
            public string Center { get; set; }
            public string Level { get; set; }
            public List<DistrictData> Districts { get; set; }
        }
    }
}
