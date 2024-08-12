using PMS.UserManage.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.UserManage.Application.ModelDto
{
    public class InterestVo : RootModel
    {
        public List<InterestItem> grade { get; set; } = new List<InterestItem>();
        public List<InterestItem> nature { get; set; } = new List<InterestItem>();
        public List<InterestItem> lodging { get; set; } = new List<InterestItem>();
    }

    public class ApiInterestVo : RootModel
    {
        public ApiInterestVo()
        {
            Interest = new ApiInterestInfoVo();
        }
        public ApiInterestInfoVo Interest { get; set; }
    }

    public class ApiInterestInfoVo
    {
        public ApiInterestInfoVo()
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
