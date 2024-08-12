using System;
using System.Collections.Generic;
using System.Text;

namespace ProductManagement.API.Http.Result
{
    public class WeChatGetTicketResult
    {
        public string appID { get; set; }
        public string appName { get; set; }
        public string ticket { get; set; }
    }
}
