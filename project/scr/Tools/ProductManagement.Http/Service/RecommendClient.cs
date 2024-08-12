using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ProductManagement.API.Http.Common;
using ProductManagement.API.Http.Interface;
using ProductManagement.API.Http.Option;
using ProductManagement.API.Http.Result;
using ProductManagement.API.Http.Result.Org;
using ProductManagement.API.Http.Result.Recommend;
using ProductManagement.Tool.HttpRequest;

namespace ProductManagement.API.Http.Service
{
    public class RecommendClient : HttpBaseClient<RecommendClientConfig>, IRecommendClient
    {
        public RecommendClient(HttpClient client, IOptions<RecommendClientConfig> config, ILoggerFactory log)
                    : base(client, config.Value, log)
        {
        }

        private async Task<RecommendResult<T>> BaseGet<T>(string url, object param)
            where T : class
        {
            return await GetAsync<RecommendResult<T>>(url, param);
        }

        private async Task<RecommendResult<T>> BasePost<T>(string url, object data)
            where T : class
        {
            try
            {
                var _url = Config.ServerUrl + url;

                string json = Newtonsoft.Json.JsonConvert.SerializeObject(data);
                HttpContent httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _client.PostAsync(_url, httpContent);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAs<RecommendResult<T>>();
            }
            catch (Exception ex)
            {
                Log.LogError(ex, null);
            }
            return default;
        }

        public async Task<IEnumerable<Guid>> RecommendSchoolIds(Guid extId, int pageIndex, int pageSize)
        {
            var url = "/Api/Recommend/GetSchoolIds";
            var param = new { eid = extId, pageIndex, pageSize };

            try
            {
                var result = await BaseGet<List<Guid>>(url, param);
                return result.Data ?? Enumerable.Empty<Guid>();
            }
            catch
            {
                return Enumerable.Empty<Guid>();
            }
        }

        public async Task<IEnumerable<Guid>> RecommendArticleIds(Guid articleId, int pageIndex, int pageSize)
        {
            var url = "/Api/Recommend/GetArticleIds";
            var param = new { aid = articleId, pageIndex, pageSize };

            try
            {
                var result = await BaseGet<List<Guid>>(url, param);
                return result.Data ?? Enumerable.Empty<Guid>();
            }
            catch
            {
                return Enumerable.Empty<Guid>();
            }
        }


        public async Task<dynamic> AddRedirectInsideSchool(string referShortId, string currentShortId)
        {
            var url = "/Api/RedirectInside/Add/School/ShortId";
            var data = new { referShortId, currentShortId };
            return await BasePost<object>(url, data);
        }

        public async Task<dynamic> AddRedirectInsideArticle(string referShortId, string currentShortId)
        {
            var url = "/Api/RedirectInside/Add/Article/ShortId";
            var data = new { referShortId, currentShortId };
            return await BasePost<object>(url, data);
        }
    }
}
