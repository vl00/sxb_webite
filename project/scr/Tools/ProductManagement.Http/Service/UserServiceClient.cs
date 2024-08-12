using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ProductManagement.API.Http.Common;
using ProductManagement.API.Http.Interface;
using ProductManagement.API.Http.Model;
using ProductManagement.API.Http.Option;
using ProductManagement.API.Http.Result;
using ProductManagement.Tool.HttpRequest;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ProductManagement.API.Http.Service
{
    public class UserServiceClient : HttpBaseClient<UserSystemConfig>, IUserServiceClient
    {
        public UserServiceClient(HttpClient client, IOptions<UserSystemConfig> config, ILoggerFactory log)
                    : base(client, config.Value, log)
        {
            //Config = config.Value;
        }
        public async Task<CollectedResult> IsCollected(IsCollected para)
        {
            CollectedSearch search = new CollectedSearch(para);
            return await GetAsync<CollectedResult, CollectedSearch>(search);
        }

        public async Task<int> PushUserMessage(AddMessage message)
        {
            Message ms = new Message(message);
            var result = await PostAsync<UserSystemResult, Message>(ms);
            return result?.status ?? -1;
        }

        public async Task<int> AddHistory(AddHistory addHistory)
        {
            if (addHistory.cookies ==null || addHistory.cookies.Count == 0)
            {
                return 0;
            }
            HistoryAdd historyAdd = new HistoryAdd(addHistory);
            var result = await GetAsync<UserSystemResult, HistoryAdd>(historyAdd);
            return result?.status ?? -1;
        }

        public async Task ReMarkDeviceToken(string userAgent, Dictionary<string,string> cookies)
        {
            try
            {
                MarkDeviceToken markDeviceToken = new MarkDeviceToken(userAgent, cookies);
                var result = await GetAsync<MarkDeviceToken>(markDeviceToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
