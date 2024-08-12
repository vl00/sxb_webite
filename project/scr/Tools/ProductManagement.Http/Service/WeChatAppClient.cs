using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ProductManagement.API.Http.Common;
using ProductManagement.API.Http.Interface;
using ProductManagement.API.Http.Model;
using ProductManagement.API.Http.Option;
using ProductManagement.API.Http.Result;
using ProductManagement.API.Http.Result.WeChatApp;
using ProductManagement.Tool.HttpRequest;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ProductManagement.API.Http.Service
{
    public class WeChatAppClient : HttpBaseClient<WeChatAppConfig>, IWeChatAppClient
    {
        public WeChatAppClient(HttpClient client, IOptions<WeChatAppConfig> config, ILoggerFactory log) : base(client, config.Value, log)
        {
        }

        public async Task<GetAccessTokenResult> GetAccessToken(WeChatGetAccessTokenRequest request)
        {
            var response = await GetAsync<WeChatBaseResponseResult<GetAccessTokenResult>, WeChatAppGetAccessTokenOption>(new WeChatAppGetAccessTokenOption(request.App));
            if (response != null && response.success)
            {
                return response.data;
            }
            else
            {
                throw new Exception(response.msg);
            }
        }

        public async Task<GetAccessTokenResult> GetAccessToken(string appname = "fwh")
        {
            var response = await GetAsync<WeChatBaseResponseResult<GetAccessTokenResult>, WeChatAppGetAccessTokenOption>(new WeChatAppGetAccessTokenOption(appname));
            if (response != null && response.success)
            {
                return response.data;
            }
            else
            {
                throw new Exception(response.msg);
            }
        }



        public async Task<bool> GetSubscribeStatus(string openID, string appName = "fwh")
        {
            var response = await GetAsync<WeChatBaseResponseResult<bool>, WeChatAppGetSubscribeStatusOption>(new WeChatAppGetSubscribeStatusOption(openID, appName));
            if (response?.success == true)
            {
                return response.data;
            }
            else
            {
                throw new Exception(response.msg);
            }
        }

        public async Task<WeChatGetTicketResult> GetTicket(WeChatGetTicketRequest request)
        {
            var response = await GetAsync<WeChatBaseResponseResult<WeChatGetTicketResult>, WeChatAppGetTicketOption>(new WeChatAppGetTicketOption(request.App));
            if (response != null && response.success)
            {
                return response.data;
            }
            else
            {
                throw new Exception(response.msg);
            }
        }
    }
}
