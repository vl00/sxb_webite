using System.Collections.Generic;

namespace Sxb.Web.Areas.School.Models.School
{
    public class GetHighSchoolAchievementResponse
    {
        public IEnumerable<dynamic> Local { get; set; }
        public IEnumerable<dynamic> OutSide { get; set; }
    }
}
