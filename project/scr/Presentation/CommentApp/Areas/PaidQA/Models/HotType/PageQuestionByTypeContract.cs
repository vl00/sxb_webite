using System;

namespace Sxb.Web.Areas.PaidQA.Models.HotType
{
    public class PageQuestionByTypeRequest
    {
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public Guid HotTypeID { get; set; }
        public int SortType { get; set; } = 1;
    }
    public class PageQuestionByTypeResponse
    {
        public string HotTypeName { get; set; }
        public string Question { get; set; }
        public string Reply { get; set; }
        public int ViewCount { get; set; }
        public string NickName { get; set; }
        public string AuthName { get; set; }
        public string Introduction { get; set; }
        public Guid TalentUserID { get; set; }
        public string HeadImgUrl { get; set; }
        public Guid OrderID { get; set; }
        public DateTime CreateTime { get; set; }
        public int TalentRole { get; set; }
    }
}
