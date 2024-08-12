using PMS.School.Application.IServices;
using PMS.School.Domain.Entities.WechatDemo;
using PMS.School.Domain.IRepositories;
using ProductManagement.Framework.Cache.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMS.School.Application.Services
{
    public class OnlineSchoolExtLevelService : ApplicationService<OnlineSchoolExtLevelInfo>, IOnlineSchoolExtLevelService
    {
        IOnlineSchoolExtLevelRepository _onlineSchoolExtLevelRepository;
        IEasyRedisClient _easyRedisClient;
        public OnlineSchoolExtLevelService(IOnlineSchoolExtLevelRepository onlineSchoolExtLevelRepository, IEasyRedisClient easyRedisClient) : base(onlineSchoolExtLevelRepository)
        {
            _easyRedisClient = easyRedisClient;
            _onlineSchoolExtLevelRepository = onlineSchoolExtLevelRepository;
        }

        public async Task<IEnumerable<OnlineSchoolExtLevelInfo>> GetByCityCodeSchFType(int cityCode, string schFType)
        {
            if (cityCode < 100000 || string.IsNullOrWhiteSpace(schFType)) return null;
            var finds = await _easyRedisClient.GetOrAddAsync($"OnlineSchoolExtLevelInfo:{cityCode}-{schFType}", async () =>
            {
                return await _onlineSchoolExtLevelRepository.GetByAsync("[CityCode] = @cityCode AND [SchFType] = @schFType", new { cityCode, schFType });
            }, TimeSpan.FromDays(1));
            if (finds?.Any() == true) return finds;
            return null;
        }

        public async Task<IEnumerable<OnlineSchoolExtLevelInfo>> GetByGrade(int grade)
        {
            if (grade < 1) return null;
            var finds = await _easyRedisClient.GetOrAddAsync("OnlineSchoolExtLevelInfo", () =>
            {
                return _onlineSchoolExtLevelRepository.GetBy("1 = 1", new { });
            }, TimeSpan.FromDays(1));
            if (finds?.Any() == true) return finds.Where(p => p.Grade == grade);
            return null;
        }

        public async Task<bool> RemoveByCityCodeSchFType(int cityCode, string schFType)
        {
            if (cityCode < 100000 || string.IsNullOrWhiteSpace(schFType)) return false;
            return await _onlineSchoolExtLevelRepository.DeleteByCityCodeSchFType(cityCode, schFType);
        }
    }
}
