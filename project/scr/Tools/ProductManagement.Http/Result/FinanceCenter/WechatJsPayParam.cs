using System;
using System.Collections.Generic;
using System.Text;

namespace ProductManagement.API.Http.Result.FinanceCenter
{

    public class WechatJsPayParam
    {
        public string AppId { get; set; }
        public string TimeStamp { get; set; }
        public string NonceStr { get; set; }
        public string SignType { get; set; }
        public string PaySign { get; set; }
        public string Package { get; set; }
    }

}
