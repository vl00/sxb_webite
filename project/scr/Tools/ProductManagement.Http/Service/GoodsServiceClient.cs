using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PMS.OperationPlateform.Domain.Entitys;
using ProductManagement.API.Http.Common;
using ProductManagement.API.Http.Interface;
using ProductManagement.API.Http.Result.Org;
using ProductManagement.Tool.HttpRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ProductManagement.API.Http.Service
{
    public class GoodsServiceClient: HttpBaseClient<OrgClientConfig>, IGoodsServiceClient
    {

        public GoodsServiceClient(HttpClient client, IOptions<OrgClientConfig> config, ILoggerFactory log) : base(client, config.Value, log)
        {
            base._client.BaseAddress = new Uri(config.Value.ServerUrl);
        }
        public async Task<GoodsObject> GetCourses(IEnumerable<string> sIds)
        {
            string url = "/api/ToSchools/infos/courses";
            var postData = new { ids = new List<string>(), sIds };
            var result = await OrgPostJson<GoodsObject>(url, postData);
            return result;
        }

        public async Task<T> OrgPostJson<T>(string url, object data)
            where T : class
        {
            try
            {
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(data);
                HttpContent httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _client.PostAsync(url, httpContent);
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadAs<BaseResult<T>>();
                if (result != null && result.succeed)
                {
                    return result.data;
                }
            }
            catch (Exception ex)
            {
                Log.LogError(ex, null);
            }
            return default;
        }

    }
}
