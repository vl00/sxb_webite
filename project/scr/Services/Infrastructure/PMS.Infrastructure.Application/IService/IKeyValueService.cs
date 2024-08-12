using System.Threading.Tasks;

namespace PMS.Infrastructure.Application.IService
{
    public interface IKeyValueService
    {
        Task<string> GetValueFromCache(string key);
        Task<string> GetValue(string key);
    }
}
