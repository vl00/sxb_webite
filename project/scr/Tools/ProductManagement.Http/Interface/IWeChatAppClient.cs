using ProductManagement.API.Http.Model;
using ProductManagement.API.Http.Result;
using ProductManagement.API.Http.Result.WeChatApp;
using System.Threading.Tasks;

namespace ProductManagement.API.Http.Interface
{
    public interface IWeChatAppClient
    {
        /// <summary>
        /// 获取微信公众号Ticket
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<WeChatGetTicketResult> GetTicket(WeChatGetTicketRequest request);

        /// <summary>
        /// 获取微信公众号AccessToken
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<GetAccessTokenResult> GetAccessToken(WeChatGetAccessTokenRequest request);

        /// <summary>
        /// 获取微信公众号AccessToken
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<GetAccessTokenResult> GetAccessToken(string appname = "fwh");

        /// <summary>
        /// 获取用户是否关注
        /// </summary>
        /// <param name="appName">服务号</param>
        /// <param name="openID">公众号关注获得的openid</param>
        /// <returns></returns>
        Task<bool> GetSubscribeStatus(string openID, string appName = "fwh");


    }
}
