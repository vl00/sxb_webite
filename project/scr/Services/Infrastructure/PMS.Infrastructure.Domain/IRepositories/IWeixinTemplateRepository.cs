using System;
using System.Threading.Tasks;

namespace PMS.Infrastructure.Domain.IRepositories
{
    public interface IWeixinTemplateRepository
    {
        Task<string> GetTemplateText(Guid key);
    }
}
