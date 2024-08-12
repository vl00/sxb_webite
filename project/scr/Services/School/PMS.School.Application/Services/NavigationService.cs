using Org.BouncyCastle.Crypto.Modes;
using PMS.School.Application.IServices;
using PMS.School.Domain.Dtos;
using PMS.School.Domain.IRepositories;
using ProductManagement.Framework.Cache.Redis;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace PMS.School.Application.Services
{
    public class NavigationService : INavigationService
    {
        INavigationRepository _navigationRepository;
        IEasyRedisClient _easyRedisClient;
        public NavigationService(INavigationRepository navigationRepository, IEasyRedisClient easyRedisClient)
        {
            _easyRedisClient = easyRedisClient;
            _navigationRepository = navigationRepository;
        }
        public async Task<IEnumerable<NavigationDto>> GetNavigations(int count = 8)
        {
            if (count < 1) return null;

            var finds = await _easyRedisClient.GetOrAddAsync("Home:IndexNavigations", () =>
             {
                 var navigations = _navigationRepository.GetList(count);
                 if (navigations.Result?.Any() == true)
                 {
                     return navigations.Result;
                 }
                 return new List<NavigationDto>();
             }, TimeSpan.FromHours(6));

            return finds;
        }

        public async Task<IEnumerable<PCNavigationDto>> GetPCNavigations()
        {
            var finds = await _navigationRepository.GetPCList();
            if (finds?.Any() == true)
            {
                foreach (var item in finds)
                {
                    item.Items = item.Items.OrderBy(p => p.ParentID).ThenBy(p => p.Index);
                }
                return finds;
            }
            return null;
        }
    }
}
