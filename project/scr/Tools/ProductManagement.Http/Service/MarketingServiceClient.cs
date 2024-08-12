using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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

    public class MarketingServiceClient : HttpBaseClient<MarketingClientConfig>, IMarketingServiceClient
    {
        public MarketingServiceClient(HttpClient client, IOptions<MarketingClientConfig> config, ILoggerFactory log) : base(client, config.Value, log)
        {
            base._client.BaseAddress = new Uri(config.Value.ServerUrl);
        }


        public async Task LockFans(Guid fansUserId, Guid headUserId, int channel)
        {
            try
            {
                    string url = $"/api/FxUser/PreLockFans";
                    var response = await _client.PostAsync(url, new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(new
                    {
                        fansUserId = fansUserId,
                        headUserId = headUserId,
                        channel = channel
                    }), Encoding.UTF8, "application/json"));
                    var responseStr = await response.Content.ReadAsStringAsync();
                    var responseObj = JObject.Parse(responseStr);
                    if (!responseObj["succeed"].Value<bool>())
                    {
                        base.Log.LogWarning(responseObj["msg"].Value<string>());
                    }
            }
            catch (Exception ex)
            {
                base.Log.LogError(ex, null);
            }
        }
    }
}
