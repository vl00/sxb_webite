using System;

namespace Sxb.Web.Areas.PaidQA.Models.Evaluate
{
    public class PageRequest
    {
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public Guid TalentUserID { get; set; }
        public Guid? TagID { get; set; }
    }
}
