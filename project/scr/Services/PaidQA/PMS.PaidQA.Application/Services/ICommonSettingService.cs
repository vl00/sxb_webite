using PMS.PaidQA.Domain.Entities;
using System.Threading.Tasks;
using System.Collections.Generic;
namespace PMS.PaidQA.Application.Services
{
    public interface ICommonSettingService : IApplicationService<CommonSetting>
    {
        /// <summary>
        /// 根据key模糊查询
        /// </summary>
        /// <param name="keyLike"></param>
        /// <returns></returns>
        public IEnumerable<CommonSetting> GetByKeyLikes(string keyLike);
    }
}
