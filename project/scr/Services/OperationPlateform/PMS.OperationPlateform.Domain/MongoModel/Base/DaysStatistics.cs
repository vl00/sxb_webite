using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.OperationPlateform.Domain.MongoModel.Base
{
    public class DaysStatistics
    {
        public DateTime? date { get; set; }
        public string url { get; set; }
        public int pv { get; set; }
        public int uv { get; set; }
    }
}
