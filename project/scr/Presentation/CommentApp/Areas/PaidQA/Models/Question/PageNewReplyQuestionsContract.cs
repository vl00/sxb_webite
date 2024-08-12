using System;

namespace Sxb.Web.Areas.PaidQA.Models.Question
{
    public class PageNewReplyQuestionsRequest
    {
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
    public class PageNewReplyQuestionsResponse
    {
        public string HeadImgUrl { get; set; }
        public string NickName { get; set; }
        public string Content { get; set; }
        public int Score { get; set; }
        public Guid TalentUserID { get; set; }
    }
}
