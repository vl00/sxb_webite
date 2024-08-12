using ProductManagement.API.Http.Model;
using ProductManagement.API.Http.Result;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ProductManagement.API.Http.Interface
{
    public interface IUserServiceClient
    {
        Task<CollectedResult> IsCollected(IsCollected para);
        Task<int> PushUserMessage(AddMessage message);
        Task<int> AddHistory(AddHistory addHistory);

        //透传cookie,记录devicetoken
        Task ReMarkDeviceToken(string userAgent, Dictionary<string, string> cookies);
    }
}
