using System;
using System.Collections.Generic;
using System.Text;

namespace ProductManagement.API.Http.Result
{
    public class GetSchoolByNameAndAdCode
    {
        public GetSchoolByNameAndAdCode()
        {
            curpage = 1;
            countperpage = 20;
        }

        public string name { get; set; }
        public int citycode { get; set; }
        public int curpage { get; set; }
        public int countperpage { get; set; }
    }
}
