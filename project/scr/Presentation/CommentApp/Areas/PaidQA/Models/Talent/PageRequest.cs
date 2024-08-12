using System;

namespace Sxb.Web.Areas.PaidQA.Models.Talent
{
    public class PageRequest
    {
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public Guid? GradeID { get; set; }
        public Guid? RegionTypeID { get; set; }
        public int OrderTypeID { get; set; }
        public Guid? LevelID { get; set; }
        public string NickName { get; set; }
        public string MinPrice { get; set; }
        public string MaxPrice { get; set; }
    }
}
