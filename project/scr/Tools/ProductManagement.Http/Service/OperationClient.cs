using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ProductManagement.API.Http.Common;
using ProductManagement.API.Http.Interface;
using ProductManagement.API.Http.Option;
using ProductManagement.API.Http.Result;
using ProductManagement.Tool.HttpRequest;
using ProductManagement.Tool.HttpRequest.Option;

namespace ProductManagement.API.Http.Service
{
    public class OperationClient: HttpBaseClient<OperationConfig>, IOperationClient
    {
        public OperationConfig Config;
        public OperationClient(HttpClient client, IOptions<OperationConfig> config, ILoggerFactory log)
                    : base(client, config.Value, log)
        {
            //Config = config.Value;
            Config = config.Value;
        }

        public async Task<RankResult> GetRankByAfterDate(DateTime afterDate,int pageNo ,int pageSize)
        {
            return await PostAsync<RankResult, GetRankByAfterDateOption>(new GetRankByAfterDateOption(afterDate,  pageNo,  pageSize) );
        }

        public async Task<SMSAPIResult> SMSApi(string templateId,string[] phones,string[] templateParams) 
        {
            var option = new SMSParam(templateId, phones, templateParams);
            option.AddHeader("contenttype", "application/json; charset=utf-8");
            return await PostAsync<SMSAPIResult, SMSParam>(option);
        }

        /// <summary>
        /// https://localhost:5001/LinkTransfer/ToShortUrl?originUrl=http://docker.sxkid.com:8080/
        /// </summary>
        /// <param name="originUrl"></param>
        /// <returns></returns>
        public async Task<OperationResult<string>> GetShortUrl(string originUrl)
        {
            var option = new ShortUrlOption(originUrl);
            return await PostAsync<OperationResult<string>, ShortUrlOption>(option);
        }

        public string GetServiceUrl()
            => Config.ServerUrl;
    }
}
