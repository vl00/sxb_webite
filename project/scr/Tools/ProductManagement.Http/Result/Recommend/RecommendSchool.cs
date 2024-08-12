using System;
using System.Collections.Generic;
using System.Text;
using static ProductManagement.API.Http.Result.RankResult;

namespace ProductManagement.API.Http.Result.Recommend
{
    public class RecommendSchool
    {
        public double Score { get; set; }
        public string Remark { get; set; }

        public School SchoolP { get; set; }
        public School SchoolS { get; set; }
    }
}
