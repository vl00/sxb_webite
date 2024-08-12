using System;

namespace Sxb.Web.Areas.PaidQA.Models.HotType
{
    public class PageRequest
    {
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public bool Random { get; set; }
        public bool OnePageRandom { get; set; }
        public Guid? HotTypeID { get; set; }
    }

    public class PageResponse
    {
        public string HeadImgUrl { get; set; }
        public string NickName { get; set; }
        public Guid TalentUserID { get; set; }
        public string Content { get; set; }
        public int Score { get; set; }
    }
}
