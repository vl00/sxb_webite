using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ProductManagement.Tool.HttpRequest;
using ProductManagement.UserCenter.BaiduCommon.Common;
using ProductManagement.UserCenter.BaiduCommon.Model;
using ProductManagement.UserCenter.BaiduCommon.Option;

namespace ProductManagement.UserCenter.BaiduCommon
{
    public class BaiduOAuthClient: HttpBaseClient<BaiduConfig>,  IBaiduOAuthClient
    {
        private readonly BaiduConfig _config;

        public BaiduOAuthClient(
            HttpClient httpClient, IOptions<BaiduConfig> config, ILoggerFactory log)
                    : base(httpClient, config.Value, log)
        {
            _config = config.Value;
        }

        /// <summary>
        /// 通过code获取access_key 和 OpenID
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="appSecret"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public async Task<BDAccessKeyResult> GetBaiduAccessKey(string code)
        {
            //string url = "https://spapi.baidu.com/oauth/jscode2sessionkey";
            var option = new AccessKeyOption(code, _config.AppID, _config.AppSecret);
            return await PostAsync<BDAccessKeyResult, AccessKeyOption>(option);
        }
    }
}
