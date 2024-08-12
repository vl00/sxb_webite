using System;
using System.Collections.Generic;
namespace Sxb.Web.Areas.PaidQA.Models.Talent
{
    public class PageParamsResponse
    {
        public IEnumerable<object> Grades { get; set; }
        public IEnumerable<object> RegionTypes { get; set; }
        public IEnumerable<object> OrderTypes { get; set; }
        public IEnumerable<object> TalentLevels { get; set; }
    }
}
