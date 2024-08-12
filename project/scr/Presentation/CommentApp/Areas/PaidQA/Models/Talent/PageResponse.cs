using System;
using System.Collections.Generic;

namespace Sxb.Web.Areas.PaidQA.Models.Talent
{
    public class PageResponse
    {
        public string HeadImgUrl { get; set; }
        public string NickName { get; set; }
        public IEnumerable<string> GradeNames { get; set; }
        public IEnumerable<string> RegionTypeNames { get; set; }
        public int Score { get; set; }
        public string Price { get; set; }
        public string ReplyPercent { get; set; }
        public string LevelName { get; set; }
        public string AuthName { get; set; }
        public Guid TalentUserID { get; set; }
        public int TalentType { get; set; }
    }
}
