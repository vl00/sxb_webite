using System;
using ProductManagement.Tool.HttpRequest.Option;

namespace ProductManagement.UserCenter.BaiduCommon.Option
{
    public class AccessKeyOption: BaseOption
    {
        public AccessKeyOption(string code,string appId, string appSecret )
        {
            AddValue("code", new RequestValue(code));
            AddValue("client_id", new RequestValue(appId));
            AddValue("sk", new RequestValue(appSecret));
        }

        public override string UrlPath => "https://spapi.baidu.com/oauth/jscode2sessionkey";
    }
}
