using PMS.PaidQA.Domain.Entities;
using System.Threading.Tasks;

namespace PMS.PaidQA.Repository
{
    public interface ICommonSettingRepository : IRepository<CommonSetting>
    {
        Task<int> Count(string str_Where, object param);
    }
}
