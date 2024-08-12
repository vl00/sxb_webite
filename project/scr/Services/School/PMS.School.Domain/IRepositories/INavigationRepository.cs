using PMS.School.Domain.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PMS.School.Domain.IRepositories
{
    public interface INavigationRepository
    {
        /// <summary>
        /// 根据个数获取导航
        /// </summary>
        /// <param name="count">个数</param>
        /// <returns></returns>
        Task<IEnumerable<NavigationDto>> GetList(int count = 8);

        /// <summary>
        /// 获取PC导航
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<PCNavigationDto>> GetPCList();
    }
}
