using System;
using System.Collections.Generic;

namespace Sxb.Web.Areas.Org.Model
{
    public class CoursesAndOrgs
    {
        public DateTime Time { get; set; }
        public IEnumerable<Course> HotSellCourses { get; set; }
        public IEnumerable<Org> RecommendOrgs { get; set; }
    }
    public class Course
    {
        public Guid ID { get; set; }
        public string ID_S { get; set; }
        public string OrgName { get; set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public int Subject { get; set; }
        public string Banner { get; set; }
        public string Price { get; set; }
        public string OrigPrice { get; set; }
        public bool Authentication { get; set; }
    }

    public class Org
    {
        public Guid ID { get; set; }
        public string ID_S { get; set; }
        public string Name { get; set; }
        public string Logo { get; set; }
        public bool Authentication { get; set; }
        public string Desc { get; set; }
        public string SubDesc { get; set; }
        public int CourceCount { get; set; }
        public int EvaluationCount { get; set; }
    }
}
