using System;
using System.Threading.Tasks;
using ProductManagement.API.Http.Result;

namespace ProductManagement.API.Http.Interface
{
    public interface IOperationClient
    {
        Task<RankResult> GetRankByAfterDate(DateTime afterDate, int pageNo, int pageSize);

        Task<SMSAPIResult> SMSApi(string templateId, string[] phones, string[] templateParams);

        string GetServiceUrl();
        Task<OperationResult<string>> GetShortUrl(string originUrl);
    }
}
