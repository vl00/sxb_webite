using System;
namespace PMS.Search.Domain.Entities
{
    public class SearchSchoolScore
    {
        public double? Composite { get; set; }//15综合 1 22
        public double? Teach { get; set; }//16师资 1 22
        public double? Hard { get; set; }//17硬件 1 22
        public double? Course { get; set; }//18课程 1 22
        public double? Learn { get; set; }//19学术 1 22
        public double? Cost { get; set; }//20费用 1 22
        public double? Envir { get; set; }//21周边 1 22
        public double? Total { get; set; }//22 总分
    }
}
