using System;
using System.Collections.Generic;
using System.Text;

namespace ProductManagement.API.Http.Result.Live
{
    public class LiveActivityExistCustomerChannelPhoneResult
    {
        public int Status {get;set;}
        public string ErrorDescription { get;set;}
        public bool Exists => Status != 0;
    }
}
