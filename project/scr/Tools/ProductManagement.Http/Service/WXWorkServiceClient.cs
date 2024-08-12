using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using ProductManagement.API.Http.Common;
using ProductManagement.API.Http.Interface;
using ProductManagement.API.Http.Model.Org;
using ProductManagement.API.Http.Request.Org;
using ProductManagement.API.Http.Request.WXWork;
using ProductManagement.API.Http.Result.Org;
using ProductManagement.Tool.HttpRequest;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ProductManagement.API.Http.Service
{

    public class WXWorkServiceClient : HttpBaseClient<WXWorkClientConfig>, IWXWorkServiceClient
    {
        public WXWorkServiceClient(HttpClient client, IOptions<WXWorkClientConfig> config, ILoggerFactory log) : base(client, config.Value, log)
        {
            base._client.BaseAddress = new Uri(config.Value.ServerUrl);
        }

        public async Task<string> GetAddCustomerQrCode(GetAddCustomerQrCodeRequest request)
        {
            var response = await base._client.PostAsync("/api/ShopActivity/GetAddCustomerQrCode", request);
            var result = await response.Content.ReadAsDynamic();
            if (result.succeed)
            {
                return result.data.qrcodeUrl;
            }
            return "";
        }
    }
}
