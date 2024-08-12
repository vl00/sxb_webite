using System.Threading.Tasks;

namespace PMS.Infrastructure.Domain.IRepositories
{
    public interface IKeyValyeRepository
    {
        Task<string> GetValue(string key);
    }
}
