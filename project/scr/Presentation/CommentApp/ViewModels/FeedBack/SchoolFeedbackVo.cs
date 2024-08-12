using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Web.ViewModels.FeedBack
{
    public class SchoolFeedbackVo
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public int Grade { get; set; }
        public int Type { get; set; }
        public double Lng { get; set; }
        public double Lat { get; set; }
        public string GradeName { get; set; }
    }
}
