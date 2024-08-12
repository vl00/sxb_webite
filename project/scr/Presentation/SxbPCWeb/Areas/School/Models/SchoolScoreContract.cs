using System.Collections.Generic;
namespace Sxb.PCWeb.Areas.School.Models
{
    public class SchoolScoreResponse
    {
        public IndexItem CurrentIndex { get; set; }
        public IList<SchoolScoreResponse> SubItems { get; set; }
        public double? Score { get; set; }
    }

    public class IndexItem
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int ParentID { get; set; }
    }
}
