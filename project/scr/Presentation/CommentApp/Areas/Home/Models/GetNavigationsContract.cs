using PMS.School.Domain.Enum;
using System.Collections.Generic;

namespace Sxb.Web.Areas.Home.Models
{
    public class GetNavigationsResponse
    {
        public string Name { get; set; }
        public int Index { get; set; }
        public string IconUrl { get; set; }
        public int Type { get; set; }
        public IEnumerable<NavigationItem> Items { get; set; }
        public IEnumerable<RecommendItem> RecommendItems { get; set; }
    }

    public class NavigationItem
    {
        public string Name { get; set; }
        public int Index { get; set; }
        public string Url { get; set; }
        public IEnumerable<NavigationItem> SubItems { get; set; }
    }

    public class RecommendItem
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public string ImgUrl { get; set; }
        public int Index { get; set; }
        public PCNavigationRecommendType Type { get; set; }
        public int Score { get; set; }
    }
}
