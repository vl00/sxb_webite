using ProductManagement.Tool.HttpRequest.Option;

namespace ProductManagement.API.Http.Option
{
    public class WeChatAppGetSubscribeStatusOption : BaseOption
    {
        public string App { get; private set; }
        public string OpenID { get; private set; }
        /// <summary>
        /// app的名称，上学帮内部定义，如果fwh表示上学帮服务号，cd表示上学帮成都订阅号
        /// </summary>
        /// <param name="app"></param>
        public WeChatAppGetSubscribeStatusOption(string openID, string app = "fwh")
        {
            App = app;
            OpenID = openID;
        }
        public override string UrlPath => $"/api/user/IsSubscribe?app={App}&openid={OpenID}";
    }
}
