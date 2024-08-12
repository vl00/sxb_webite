using System;
using System.Collections.Generic;
using System.Text;

namespace iSchool.Internal.API.UserModule.Models
{

    public class SchoolIDs :ResponseModel{

        public Guid[] data { get; set; }
    }

    public class ApiInterest : ResponseModel
    {
        public ApiInterest()
        {
            interest = new ApiInterestInfo();
        }
        public ApiInterestInfo interest { get; set; }
    }


    public class ApiInterestInfo
    {
        public ApiInterestInfo()
        {
            grade = new List<int>();
            nature = new List<int>();
            lodging = new List<int>();
        }
        public List<int> grade { get; set; }
        public List<int> nature { get; set; }
        public List<int> lodging { get; set; }
    }

}
