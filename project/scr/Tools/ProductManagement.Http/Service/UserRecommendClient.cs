using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ProductManagement.API.Http.Common;
using ProductManagement.API.Http.Interface;
using ProductManagement.API.Http.Option;
using ProductManagement.API.Http.Result;
using ProductManagement.Tool.HttpRequest;

namespace ProductManagement.API.Http.Service
{
    public class UserRecommendClient: HttpBaseClient<UserRecommendConfig>, IUserRecommendClient
    {
        public UserRecommendClient(HttpClient client, IOptions<UserRecommendConfig> config, ILoggerFactory log)
                    : base(client, config.Value, log)
        {
            //Config = config.Value;
        }

        public async Task<BaseResponseResult<List<UserRecommendResult>>> UserRecommendSchool(Guid userId,int index,int size)
        {
            var result = await PostAsync<BaseResponseResult<List<UserRecommendResult>>, UserRecommendOption>(new UserRecommendOption(userId,index,size));
            return result;
        }
    }
}
