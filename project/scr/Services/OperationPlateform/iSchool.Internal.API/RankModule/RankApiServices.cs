using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace iSchool.Internal.API.RankModule
{
    public class RankApiServices
    {
        protected HttpClient client;
        readonly ILogger logger;

        public RankApiServices(
            HttpClient httpClient,
            ILogger<RankApiServices> logger
            )
        {
            this.client = httpClient;
            this.logger = logger;

        }

        //获取学校排行榜
        public async Task<string> GetTheNationwideAndNewsRanks(int city, int offset = 0, int limit = 3)
        {
            var response = await this.client.GetAsync($"/api/RankApi/GetTheNationwideAndNewsRanks?cityCode={city}&offset={offset}&limit={limit}");
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var responseResult = await response.Content.ReadAsStringAsync();
                return responseResult;
            }
            else
            {
                this.logger.LogInformation("请求工具模块时服务数据返回不正确。错误信息:{erro}", response.RequestMessage);
                return null;
            }
        }
        //获取毕业去向的国际大学榜单
        public async Task<string> GetInternationalCollegeRank()
        {
            var response = await this.client.GetAsync($"/api/Achievement/GetAchievement");
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var responseResult = await response.Content.ReadAsStringAsync();
                return responseResult;
            }
            else
            {
                this.logger.LogInformation("请求工具模块时服务数据返回不正确。错误信息:{erro}", response.RequestMessage);
                return null;
            }
        }
    }
}
