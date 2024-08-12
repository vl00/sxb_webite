using System.Collections.Generic;

namespace Sxb.Web.Areas.School.Models
{
    public class GetCommentScoreStatisticsResponse
    {
        public IEnumerable<string> HeadImgUrls { get; set; }
        public int CommentCount { get; set; }
        public string AvgScore { get; set; }
        public int AvgStars { get; set; }
        public int FivePercent { get; set; }
        public int FourPercent { get; set; }
        public int ThreePercent { get; set; }
        public int TwoPercent { get; set; }
        public int OnePercent { get; set; }
    }
}
