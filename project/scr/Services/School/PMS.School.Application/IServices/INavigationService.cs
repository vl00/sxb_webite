using PMS.School.Domain.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PMS.School.Application.IServices
{
    public interface INavigationService
    {
        /// <summary>
        /// 获取首页导航
        /// </summary>
        /// <param name="count">获取个数</param>
        /// <returns></returns>
        Task<IEnumerable<NavigationDto>> GetNavigations(int count = 8);

        Task<IEnumerable<PCNavigationDto>> GetPCNavigations();
    }
}