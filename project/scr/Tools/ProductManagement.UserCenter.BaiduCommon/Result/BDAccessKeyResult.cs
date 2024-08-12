using System;
namespace ProductManagement.UserCenter.BaiduCommon.Model
{
    public class BDAccessKeyResult
    {
        public long Errno { get; set; }
        public string Error { get; set; }
        public string Error_description { get; set; }

        public string Openid { get; set; }
        public string Session_key { get; set; }
    }
}
