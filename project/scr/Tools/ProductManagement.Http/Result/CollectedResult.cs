using System;
using System.Collections.Generic;
using System.Text;

namespace ProductManagement.API.Http.Result
{
    public class CollectedResult
    {
        public int status { get; set; }
        public bool iscollected { get; set; }
        public Guid userID { get; set; }
    }
}
