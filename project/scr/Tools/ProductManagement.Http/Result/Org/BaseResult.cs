using System;
using System.Collections.Generic;
using System.Text;

namespace ProductManagement.API.Http.Result.Org
{
    public class BaseResult<TData> where TData:class
    {
        public bool succeed { get; set; }
        public long msgTimeStamp { get; set; }
        public int status { get; set; }
        public string msg { get; set; }
        public TData data { get; set; }

    }
}
