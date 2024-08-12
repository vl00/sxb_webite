using System;
using System.Threading.Tasks;

namespace PMS.Infrastructure.Application.IService
{
    public interface IWeixinTemplateService
    {
        Task<string> GetTemplateTextFromCache(Guid id);
    }
}
