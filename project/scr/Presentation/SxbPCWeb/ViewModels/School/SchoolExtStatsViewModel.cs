using System;
using System.Collections.Generic;

namespace Sxb.PCWeb.ViewModels.School
{
    public class SchoolExtStatsViewModel
    {
        public List<SchoolExtHistViewModel> Hists { get; set; }

        public StatsUrl Url { get; set; }
    }

    public class StatsUrl
    {
        public string Origin { get; set; }
        public string Cost { get; set; }
        public string Student { get; set; }
        public string Teacher { get; set; }

        public string ScoreCompre { get; set; }
        public string ScoreTeach { get; set; }
        public string ScoreHard { get; set; }
        public string ScoreCourse { get; set; }
        public string ScoreLearn { get; set; }
        public string ScoreCost { get; set; }

        public string ScoreTotal{ get; set; }
    }

    public class SchoolExtHistViewModel
    {
        public string Name { get; set; }
        public List<SchoolExtHistCountViewModel> Hist { get; set; }
    }

    public class SchoolExtHistCountViewModel
    {
        public string Key { get; set; }
        public long Count { get; set; }
    }
}
