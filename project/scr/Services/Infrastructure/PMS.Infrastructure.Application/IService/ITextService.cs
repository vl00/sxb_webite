using System;
using System.Threading.Tasks;
using ProductManagement.API.Aliyun.Model;

namespace PMS.Infrastructure.Application.IService
{
    public interface ITextService
    {
        Task<bool> GreenTextCheck(string Keywords);
        Task<GarbageCheckResponse[]> GreenTextCheckDetail(string Keywords);
    }
}
