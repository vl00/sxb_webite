using System;
using ProductManagement.Tool.HttpRequest;

namespace ProductManagement.UserCenter.BaiduCommon.Common
{
    public class BaiduConfig : RequestConfig
    {
        public string AppID { get; set; }
        public string AppSecret { get; set; }
    }
}
