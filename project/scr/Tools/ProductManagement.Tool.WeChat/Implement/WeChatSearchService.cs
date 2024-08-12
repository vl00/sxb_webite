using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WeChat.Interface;
using WeChat.Model;

namespace WeChat.Implement
{
    public class WeChatSearchService : IWeChatSearchService
    {
        HttpClient _client;
        ILogger _logger;

        public WeChatSearchService(HttpClient client, ILogger<WeChatSearchService> logger)
        {
            _client = client;
            _logger = logger;
        }

        public async Task<string> UpsertPreUniversityBasic(string access_token, UpsertPreUniversityBasicRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException();
            }
            string url = $"https://api.weixin.qq.com/search/service/school?action=upsert_pre_university_basic&access_token={access_token}";
            StringContent stringContent = new StringContent(JsonConvert.SerializeObject(request, new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore
            }));
            string response = await this._client.PostAsync(url, stringContent, _logger);
            return response;

        }
    }
}
