using PMS.School.Domain.Entities.WechatDemo;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace PMS.School.Domain.IRepositories
{
    public interface IOnlineSchoolExtLevelRepository : IRepository<OnlineSchoolExtLevelInfo>
    {
        /// <summary>
        /// 根据城市代码与学校类型删除
        /// </summary>
        /// <returns></returns>
        Task<bool> DeleteByCityCodeSchFType(int cityCode, string schFType);
    }
}
