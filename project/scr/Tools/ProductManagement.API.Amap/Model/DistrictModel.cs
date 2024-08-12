using System;
namespace ProductManagement.Tool.Amap.Model
{
    public class DistrictModel
    {
        public string Keywords { get; set; }
        public int Subdistrict { get; set; } = 1;
        public int Page { get; set; } = 1;
        public int Offset { get; set; } = 20;
        public string Extensions { get; set; }
        public string Filter { get; set; }
        public string Callback { get; set; }
        public string Output { get; set; }
    }
}
