using PMS.School.Domain.Entities.WechatDemo;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PMS.School.Application.IServices
{
    public interface IOnlineSchoolExtLevelService : IApplicationService<OnlineSchoolExtLevelInfo>
    {
        Task<IEnumerable<OnlineSchoolExtLevelInfo>> GetByGrade(int grade);
        /// <summary>
        /// 根据城市代码与学校类型获取
        /// </summary>
        /// <returns></returns>
        Task<bool> RemoveByCityCodeSchFType(int cityCode, string schFType);
        /// <summary>
        /// 根据城市代码与学校类型删除
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<OnlineSchoolExtLevelInfo>> GetByCityCodeSchFType(int cityCode, string schFType);
    }
}
