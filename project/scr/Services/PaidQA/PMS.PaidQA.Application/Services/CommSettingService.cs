using PMS.PaidQA.Domain.Entities;
using PMS.PaidQA.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PMS.PaidQA.Application.Services
{
    public class CommSettingService : ApplicationService<CommonSetting>, ICommonSettingService
    {
        ICommonSettingRepository _commonSettingRepository;
        public CommSettingService(ICommonSettingRepository commonSettingRepository) : base(commonSettingRepository)
        {
            _commonSettingRepository = commonSettingRepository;
        }

        public IEnumerable<CommonSetting> GetByKeyLikes(string keyLike)
        {
            if (string.IsNullOrWhiteSpace(keyLike)) return null;
            var str_Where = $"[key] like '%{keyLike}%'";
            return _commonSettingRepository.GetBy(str_Where, new { },fileds:new string[3] { "ID","[Key]","[Value]"});
        }
    }
}
