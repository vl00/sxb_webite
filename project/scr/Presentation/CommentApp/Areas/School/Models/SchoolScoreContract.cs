using System.Collections.Generic;
namespace Sxb.PC.Areas.School.Models
{
    public class SchoolScoreResponse
    {
        public IndexItem CueentIndex { get; set; }
        public IList<SchoolScoreResponse> SubItems { get; set; }
        public double? Score { get; set; }
        public string Str_Score
        {
            get
            {
                if (Score.HasValue)
                {
                    if (Score.Value >= 90)
                    {
                        return "A+";
                    }
                    else if (Score.Value >= 80 && Score.Value < 90)
                    {
                        return "A";
                    }
                    else if (Score.Value >= 70 && Score.Value < 80)
                    {
                        return "B";
                    }
                    else if (Score.Value >= 60 && Score.Value < 70)
                    {
                        return "C";
                    }
                    else
                    {
                        return "D";
                    }
                }
                return string.Empty;
            }
        }
    }

    public class IndexItem
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int ParentID { get; set; }
    }
}
