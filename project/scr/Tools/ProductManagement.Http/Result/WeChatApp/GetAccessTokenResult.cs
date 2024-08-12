using System;
using System.Collections.Generic;
using System.Text;

namespace ProductManagement.API.Http.Result.WeChatApp
{
    public class GetAccessTokenResult
    {
        public string appID { get; set; }
        public string appName { get; set; }
        public string token { get; set; }

    }
}
