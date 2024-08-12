using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.School.Domain.Common
{
    public class ApiInterest
    {
        public int status { get; set; }
        public string errorDescription { get; set; }
        public ApiInterestInfo interest { get; set; }
    }
    public class ApiInterestInfo
    {
        public int[] grade { get; set; }
        public int[] nature { get; set; }
        public int[] lodging { get; set; }
    }
}
