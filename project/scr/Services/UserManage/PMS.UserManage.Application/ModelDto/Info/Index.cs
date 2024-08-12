using PMS.UserManage.Domain.Common;
using PMS.UserManage.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMS.UserManage.Application.ModelDto.Info
{
    public class Index: RootModel
    {
        public UserInfo UserInfo { get; set; }
        public Interest Interest { get; set; }
    }
    public class Interest : RootModel
    {
        public List<InterestItem> grade { get; set; } = new List<InterestItem>();
        public List<InterestItem> nature { get; set; } = new List<InterestItem>();
        public List<InterestItem> lodging { get; set; } = new List<InterestItem>();
    }

    public class ApiInterest : RootModel
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
