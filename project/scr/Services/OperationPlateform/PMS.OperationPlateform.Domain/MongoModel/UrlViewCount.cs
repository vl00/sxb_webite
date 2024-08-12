using System;
using System.Collections.Generic;

namespace PMS.OperationPlateform.Domain.MongoModel
{
    public class UrlViewCount
    {
        public string Url { get; set; }

        public string Fw { get; set; }
        public string Date { get; set; }
        public int Pv { get; set; }
        public int Uv { get; set; }
        public Dictionary<string, string> Params { get; set; }
    }
}
