using System;
using System.Collections.Generic;
using System.Text;

namespace ProductManagement.UserCenter.Wx.Model
{
    public class WXMiniProgramMobile
    {
        public string phoneNumber { get; set; }
        public string purePhoneNumber { get; set; }
        public string countryCode { get; set; }
        public WXMiniProgramMobileWatermark watermark { get; set; }
    }

    public class WXMiniProgramMobileWatermark
    {
        public string appid { get; set; }
        public long timestamp { get; set; }
    }
}
