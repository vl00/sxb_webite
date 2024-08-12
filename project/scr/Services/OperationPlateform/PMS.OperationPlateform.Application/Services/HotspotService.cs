using NPOI.SS.Formula.Functions;
using PMS.OperationPlateform.Application.Dtos;
using PMS.OperationPlateform.Application.IServices;
using PMS.OperationPlateform.Domain.DTOs;
using PMS.OperationPlateform.Domain.Entitys;
using PMS.OperationPlateform.Domain.Enums;
using PMS.OperationPlateform.Domain.IRespositories;
using PMS.School.Domain.IRepositories;
using PMS.Search.Domain.Entities;
using PMS.Search.Domain.IRepositories;
using PMS.UserManage.Application.IServices;
using ProductManagement.API.Http.Interface;
using ProductManagement.Framework.Cache.Redis;
using ProductManagement.Framework.Foundation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMS.OperationPlateform.Application.Services
{
    public static class HotspotGroup
    {
        public static string[] Groups { get; private set; } = new string[] { "PlaceHolderWords", "HotWords", "HotSchool" };
        public static string PlaceHolderWords = Groups[0];
        public static string HotWords = Groups[1];
        public static string HotSchool = Groups[2];
    }

    /// <summary>
    /// 热点数据
    /// </summary>
    public class HotspotService : IHotspotService
    {
        private readonly IBaseRepository<Hotspot> _hotspotRepository;
        private readonly IEasyRedisClient _easyRedisClient;

        public HotspotService(IBaseRepository<Hotspot> hotspotRepository, IEasyRedisClient easyRedisClient)
        {
            _hotspotRepository = hotspotRepository;
            _easyRedisClient = easyRedisClient;
        }

        private IEnumerable<HotspotDto> ToDto(IEnumerable<Hotspot> hotSpots)
        {
            return hotSpots.Select(s => new HotspotDto()
            {
                Id = s.Id,
                KeyName = s.KeyName,
                DataId = s.DataId,
                LinkUrl = s.LinkUrl
            });
        }

        public async Task<IEnumerable<HotspotDto>> GetPlaceholders(int cityId, ClientType clientType = ClientType.PC)
        {
            var data = await GetHotspotsCache(HotspotGroup.PlaceHolderWords, cityId, clientType);
            return ToDto(data);
        }

        public async Task<IEnumerable<HotspotDto>> GetHotWords(int cityId, ClientType clientType = ClientType.PC)
        {
            var data = await GetHotspotsCache(HotspotGroup.HotWords, cityId, clientType);
            return ToDto(data);
        }

        public async Task<IEnumerable<HotspotDto>> GetHotSchools(int cityId, ClientType clientType = ClientType.PC)
        {
            var data = await GetHotspotsCache(HotspotGroup.HotSchool, cityId, clientType);
            return ToDto(data);
        }

        /// <summary>
        /// 获取热点数据, 使用缓存
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="cityId"></param>
        /// <param name="clientType"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Hotspot>> GetHotspotsCache(string groupName, int cityId, ClientType clientType = ClientType.PC)
        {
            var key = string.Format(RedisKeys.HotspotKey, groupName, clientType, cityId);
            return await _easyRedisClient.GetOrAddEnumerableAsync(key, () =>
            {
                var hotspots = GetHotspots(groupName, cityId, clientType);
                //城市无数据, 从全国拿数据
                if (cityId != 0 && !hotspots.Any())
                {
                    cityId = 0;
                    hotspots = GetHotspots(groupName, cityId, clientType);
                }
                return hotspots;
            }, TimeSpan.FromSeconds(30));
            //}, TimeSpan.FromMinutes(30));
        }

        /// <summary>
        /// 获取热点数据
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="cityId"></param>
        /// <param name="clientType"></param>
        /// <returns></returns>
        public IEnumerable<Hotspot> GetHotspots(string groupName, int cityId, ClientType clientType = ClientType.PC)
        {
            //top 10
            var where = $@"
                    IsDeleted = 0
                    AND Status = 1
                    AND GroupName = @groupName
                    AND CityId = @cityId
                    AND ClientType = @clientType
            ";
            var data = _hotspotRepository.Select(where,
                new { groupName, cityId, clientType },
                order: " CityId ASC, Sort ASC, CreateTime DESC ",
                new string[] { "Id", "KeyName", "DataId", "LinkUrl" });
            return data;
        }
    }
}
