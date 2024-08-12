using PMS.Infrastructure.Application.IService;
using PMS.Infrastructure.Application.ModelDto;
using PMS.Infrastructure.Domain;
using PMS.Infrastructure.Domain.IRepositories;
using ProductManagement.Framework.Cache.Redis;
using ProductManagement.Tool.Amap;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ProductManagement.Framework.Foundation;
using System.Linq;
using PMS.Infrastructure.Domain.Entities;
using System.Text.RegularExpressions;
using PMS.Search.Application.ModelDto.Query;

namespace PMS.Infrastructure.Application.Service
{
    public class CityInfoService : ICityInfoService
    {
        private readonly ICityCodeRepository _cityRepository;
        private readonly IMetroInfoRepository _metroRepository;

        private readonly IAmapClient _amapClient;
        private readonly IEasyRedisClient _easyRedisClient;
        public CityInfoService(IAmapClient amapClient, IEasyRedisClient easyRedisClient,
            ICityCodeRepository cityRepository, IMetroInfoRepository metroRepository)
        {
            _amapClient = amapClient;
            _easyRedisClient = easyRedisClient;
            _cityRepository = cityRepository;
            _metroRepository = metroRepository;
        }


        public List<CityCode> GetAllCityCode()
        {
            return _cityRepository.GetAllCityCode();
        }
        public async Task<List<CityCode>> GetAllCityCodes()
        {
            string Key = "City:CityCodes";
            var allCode = await _easyRedisClient.GetOrAddAsync(Key, () => { return GetAllCityCode(); }, new TimeSpan(24 * 7, 0, 0));
            return allCode;
        }

        public async Task<int> GetCityCodeByAreaCode(int areaCode)
        {
            return _cityRepository.GetAeraInfoById(areaCode)?.Parentid ?? 0;
        }


        public async Task<string> GetCityName(int cityCode)
        {
            if (cityCode <= 0) return "全国";
            string key = "City:AllCity";

            if (await _easyRedisClient.HashExistsAsync(key, cityCode.ToString()))
            {
                return await _easyRedisClient.HashGetAsync<string>(key, cityCode.ToString());
            }
            else
            {
                string cityName = null;
                var allCode = await GetAllCityCodes();

                if (!await _easyRedisClient.ExistsAsync(key))
                {
                    Dictionary<string, string> dic = allCode.ToDictionary(k => k.Id.ToString(), v => v.Name);
                    await _easyRedisClient.HashSetAsync(key, dic, StackExchange.Redis.CommandFlags.FireAndForget);
                }
                cityName = allCode.FirstOrDefault(_ => _.Id == cityCode)?.Name;

                return cityName;
            }
        }

        /// <summary>
        /// 初始化城市adcode码
        /// </summary>
        /// <returns></returns>
        public async Task<List<AdCodeDto>> IntiAdCodes()
        {
            List<AdCodeDto> adCodes = new List<AdCodeDto>();
            for (int i = 65; i <= 90; i++)
            {
                AdCodeDto adCode = new AdCodeDto
                {
                    Key = Codstring.NumToChar(i),
                    CityCodes = new List<CityCodeDto>()
                };
                adCodes.Add(adCode);
            }

            string Key = "City:AdCode";
            var allCode = await _easyRedisClient.GetOrAddAsync(Key, () => { return GetAllCityCode(); }, new TimeSpan(24 * 7, 0, 0));

            foreach (var item in allCode)
            {
                string Letter = Codstring.GetCodstring(item.Name[0].ToString());

                CityCodeDto cityCode = new CityCodeDto
                {
                    Id = item.Id,
                    Name = item.Name,
                    Parent = item.Parent
                };
                adCodes.FirstOrDefault(x => x.Key == Letter)?.CityCodes.Add(cityCode);
            }



            for (int i = 0; i < adCodes.Count; i++)
            {
                if (!adCodes[i].CityCodes.Any())
                {
                    adCodes.Remove(adCodes[i]);
                }
            }
            return adCodes;
        }



        public async Task<Dictionary<char, List<CitySearchDto>>> GetAllCityPinYin()
        {
            string Key = "City:AdCode";
            var allCode = await _easyRedisClient.GetOrAddAsync(Key, () => { return GetAllCityCode(); }, new TimeSpan(24 * 7, 0, 0));

            string SearchKey = "City:PinYinSearch";
            return await _easyRedisClient.GetOrAddAsync<Dictionary<char, List<CitySearchDto>>>(SearchKey, () =>
            {
                List<string> PinYin = new List<string>();
                Dictionary<char, List<CitySearchDto>> keyValuePairs = new Dictionary<char, List<CitySearchDto>>();
                foreach (var item in allCode)
                {
                    PinYin = Codstring.GetTotalPingYin(item.Name);

                    CitySearchDto citySearch = new CitySearchDto()
                    {
                        CityCode = item.Id,
                        Name = item.Name,
                        Pinyin = PinYin
                    };

                    for (int i = 0; i < PinYin.Count(); i++)
                    {
                        char CityFirstCode = PinYin[i][0];

                        if (!keyValuePairs.ContainsKey(CityFirstCode))
                        {
                            keyValuePairs.Add(CityFirstCode, new List<CitySearchDto>());
                        }

                        keyValuePairs[CityFirstCode].Add(citySearch);
                    }
                }
                return keyValuePairs;
            });
        }


        /// <summary>
        ///获取省跟城市的联动数据
        /// </summary>
        /// <returns></returns>
        public async Task<List<ProvinceCodeDto>> GetProvinceInfos()
        {
            string Key = "AllCity:AdCode";
            var allCode = await _easyRedisClient.GetOrAddAsync(Key, () => { return GetAllCityCode(); }, new TimeSpan(24 * 7, 0, 0));

            List<ProvinceCodeDto> data = new List<ProvinceCodeDto>();
            var provinces = await _easyRedisClient.GetOrAddAsync("AllProvince:AdCode",
                () => { return _cityRepository.GetProvinceInfo(); }, new TimeSpan(24 * 7, 0, 0));
            foreach (var item in provinces)
            {
                data.Add(new ProvinceCodeDto
                {
                    Province = item.Name,
                    ProvinceId = item.Id,
                    CityCodes = allCode.Where(p => p.Parent == item.Id).Select(p => new CityCodeDto
                    {
                        Id = p.Id,
                        Parent = p.Parent,
                        Name = p.Name
                    }).ToList()
                });
            }


            return data;
        }

        /// <summary>
        ///获取省份信息
        /// </summary>
        /// <returns></returns>
        public async Task<List<ProvinceCodeDto>> GetProvince()
        {
            List<ProvinceCodeDto> data = new List<ProvinceCodeDto>();
            var provinces = await _easyRedisClient.GetOrAddAsync("AllProvince:AdCode",
                () => { return _cityRepository.GetProvinceInfo(); }, new TimeSpan(24 * 7, 0, 0));
            foreach (var item in provinces)
            {
                data.Add(new ProvinceCodeDto
                {
                    Province = item.Name,
                    ProvinceId = item.Id
                });
            }
            return data;
        }
        public async Task<ProvinceCodeDto> GetProvinceInfo(int provinceCode)
        {
            string Key = "AllCity:AdCode";
            var allCode = await _easyRedisClient.GetOrAddAsync(Key, () => { return GetAllCityCode(); }, new TimeSpan(24 * 7, 0, 0));

            var provinces = await _easyRedisClient.GetOrAddAsync("AllProvince:AdCode",
                () => { return _cityRepository.GetProvinceInfo(); }, new TimeSpan(24 * 7, 0, 0));

            var province = provinces.FirstOrDefault(q => q.Id == provinceCode);

            return province == null ? null : new ProvinceCodeDto
            {
                Province = province.Name,
                ProvinceId = province.Id,
                CityCodes = allCode.Where(p => p.Parent == province.Id).Select(p => new CityCodeDto
                {
                    Id = p.Id,
                    Parent = p.Parent,
                    Name = p.Name
                }).ToList()
            };
        }

        public async Task<ProvinceCodeDto> GetProvinceInfoByCity(int cityCode)
        {
            string Key = "AllCity:AdCode";
            var allCode = await _easyRedisClient.GetOrAddAsync(Key, () => { return GetAllCityCode(); }, new TimeSpan(24 * 7, 0, 0));

            var province = _cityRepository.GetProvinceInfoByCity(cityCode);

            return province == null ? null : new ProvinceCodeDto
            {
                Province = province.Name,
                ProvinceId = province.Id,
                CityCodes = allCode.Where(p => p.Parent == province.Id).Select(p => new CityCodeDto
                {
                    Id = p.Id,
                    Parent = p.Parent,
                    Name = p.Name
                }).ToList()
            };
        }

        /// <summary>
        /// 根据城市获取区域
        /// </summary>
        /// <param name="cityName"></param>
        /// <returns></returns>
        public async Task<List<AreaDto>> GetAreaCode(string cityName)
        {
            string key = "City:Districts:" + cityName;

            var result = await _easyRedisClient.GetOrAddAsync(key, () =>
            {
                return _amapClient.GetDistrict(new ProductManagement.Tool.Amap.Model.DistrictModel
                {
                    Keywords = cityName,
                    Subdistrict = 3
                });
            }, new TimeSpan(0, 7, 0));


            if (result == null)
            {
                return null;
            }

            List<AreaDto> areas = new List<AreaDto>();
            if (cityName == "北京" || cityName == "上海" || cityName == "天津" || cityName == "重庆")
            {
                foreach (var item in result.Districts.FirstOrDefault().Districts.FirstOrDefault().Districts)
                {
                    AreaDto area = new AreaDto
                    {
                        AdCode = int.Parse(item.Adcode),
                        AreaName = item.Name
                    };
                    areas.Add(area);
                }
            }
            else
            {
                foreach (var item in result.Districts.FirstOrDefault().Districts)
                {
                    AreaDto area = new AreaDto
                    {
                        AdCode = int.Parse(item.Adcode),
                        AreaName = item.Name
                    };
                    areas.Add(area);
                }
            }
            return areas;
        }

        /// <summary>
        /// 根据城市编号获取区域
        /// </summary>
        /// <param name="cityCodes"></param>
        /// <returns></returns>
        public async Task<List<CityAreaDto>> GetAreaCode(List<int> cityCodes)
        {
            if (!cityCodes.Any())
            {
                return new List<CityAreaDto>();
            }

            string key = "City:CityArea:" + string.Join("_", cityCodes);

            var metroInfo = await _easyRedisClient.GetOrAddAsync(key, () =>
            {
                var result = _cityRepository.GetAeraInfo(cityCodes);

                return result.GroupBy(q => q.Parentid).Select(q => new CityAreaDto
                {
                    CityCode = q.Key,
                    Areas = result.Where(p => p.Parentid == q.Key).Select(p => new AreaDto
                    {
                        AdCode = p.Id,
                        AreaName = p.Name
                    }).ToList()
                }).ToList();
            }, new TimeSpan(1, 0, 0));

            return metroInfo;
        }

        /// <summary>
        /// 根据城市编号获取区域
        /// </summary>
        /// <param name="cityCode"></param>
        /// <returns></returns>
        public async Task<List<AreaDto>> GetAreaCode(int cityCode)
        {
            string key = "City:Area:" + cityCode;

            var metroInfo = await _easyRedisClient.GetOrAddAsync(key, () =>
            {
                var result = _cityRepository.GetAeraInfo(cityCode);

                return result?.Select(q => new AreaDto
                {
                    AdCode = q.Id,
                    AreaName = q.Name
                }).ToList();
            }, new TimeSpan(1, 0, 0));

            return metroInfo;
        }

        /// <summary>
        /// 根据城市编号获取地铁信息
        /// </summary>
        /// <param name="cityCode"></param>
        /// <returns></returns>
        public async Task<List<MetroDto>> GetMetroList(int cityCode)
        {
            string key = "City:Metro:" + cityCode;


            var metroInfo = await _easyRedisClient.GetOrAddAsync(key, () =>
           {
               List<MetroDto> metros = new List<MetroDto>();

               var result = _metroRepository.GetMetroInfoList(cityCode);
               if (result == null)
                   return null;
               int index = 0;
               metros = result.GroupBy(q => (q.MetroId, q.MetroName))
                   .Select(q => new MetroDto
                   {
                       MetroNo = index++,
                       MetroId = q.Key.MetroId,
                       MetroName = q.Key.MetroName,
                       MetroStations = new List<MetroDto.MetroLine>()
                   }).ToList();

               if (metros?.Any() == true)
               {
                   var regex = new Regex("[\\d]+");
                   metros = metros.OrderBy(p => int.Parse(regex.Match(p.MetroName).Success ? regex.Match(p.MetroName).Value : "99")).ToList();
               }
               foreach (var item in result)
               {
                   metros.FirstOrDefault(x => x.MetroId == item.MetroId)?.MetroStations.Add(
                       new MetroDto.MetroLine
                       {
                           Id = item.MetroLineId,
                           Name = item.MetroLineName,
                           Lat = item.Latitude,
                           Lng = item.Longitude
                       }
                   );
               }
               return metros;
           }, new TimeSpan(1, 0, 0));
            return metroInfo;
        }


        public async Task<List<MetroDto>> GetMetroList(List<Guid> metroLineIds)
        {
            string key = "City:Metros:" + DesTool.Md5(string.Join("_", metroLineIds));


            var metroInfo = await _easyRedisClient.GetOrAddAsync(key, () =>
            {
                List<MetroDto> metros = new List<MetroDto>();

                var result = _metroRepository.GetMetroInfoList(metroLineIds);
                if (result == null)
                    return null;

                metros = result.GroupBy(q => (q.MetroId, q.MetroName))
                    .Select(q => new MetroDto
                    {
                        MetroId = q.Key.MetroId,
                        MetroName = q.Key.MetroName,
                        MetroStations = new List<MetroDto.MetroLine>()
                    }).ToList();

                foreach (var item in result)
                {
                    metros.FirstOrDefault(x => x.MetroId == item.MetroId)?.MetroStations.Add(
                        new MetroDto.MetroLine
                        {
                            Id = item.MetroLineId,
                            Name = item.MetroLineName
                        }
                    );
                }
                return metros;
            }, new TimeSpan(1, 0, 0));

            return metroInfo;
        }


        public async Task<List<AreaPolylineDto>> GetAreaPolyline(int cityCode)
        {
            string Key = $"City:AreaPolyline:{cityCode}";
            var data = await _easyRedisClient.GetOrAddAsync(Key, () =>
            {
                var list = _cityRepository.GetAreaPolyline(cityCode);
                return list.Select(q => new AreaPolylineDto
                {
                    Code = q.Id,
                    Name = q.Name,
                    Latitude = Convert.ToDouble(q.Centerlocation.Split(',')[1]),
                    Longitude = Convert.ToDouble(q.Centerlocation.Split(',')[0])
                }).ToList();
            }, new TimeSpan(24, 0, 0));
            return data;
        }



        public async Task<AreaDto> GetInfoByCityName(string cityName)
        {
            string key = "CityInfo:" + cityName;
            var data = await _easyRedisClient.GetOrAddAsync(key, () =>
            {
                return _cityRepository.GetInfoByName(cityName);
            }, new TimeSpan(1, 0, 0));

            return new AreaDto() { AdCode = data.Id, AreaName = data.Name };
        }

        public async Task<AreaDto> GetInfoByCityCode(int CityCode)
        {
            string key = "CityInfo:CityCode_" + CityCode;
            var data = await _easyRedisClient.GetOrAddAsync(key, () =>
            {
                return _cityRepository.GetInfoByCityCode(CityCode);
            }, new TimeSpan(1, 0, 0));

            return new AreaDto() { AdCode = data.Id, AreaName = data.Name };
        }

        public async Task<List<AreaDto>> GetHotCity()
        {
            string Key = "SearchCity:Hot";

            var result = await _easyRedisClient.SortedSetRangeByRankAsync<AreaDto>(Key, 0, 10, StackExchange.Redis.Order.Ascending);

            if (result == null || !result.Any())
            {
                List<AreaDto> areas = new List<AreaDto>
                {
                    new AreaDto { AreaName = "北京", AdCode = 110100 },
                    new AreaDto { AreaName = "上海", AdCode = 310100 },
                    new AreaDto { AreaName = "广州", AdCode = 440100 },
                    new AreaDto { AreaName = "深圳", AdCode = 440300 },
                    new AreaDto { AreaName = "重庆", AdCode = 500100 },
                    new AreaDto { AreaName = "成都", AdCode = 510100 },
                    new AreaDto { AreaName = "佛山", AdCode = 440600 }
                };
                result = areas;
                Dictionary<AreaDto, double> pairs = new Dictionary<AreaDto, double>();

                int i = 0;
                foreach (var item in areas.ToList())
                {
                    pairs.Add(item, i++);
                }
                await _easyRedisClient.SortedSetAddAsync<AreaDto>(Key, pairs, StackExchange.Redis.CommandFlags.FireAndForget);
            }
            if (result?.Count() > 0)
            {
                result = result.GroupBy(p => p.AdCode).Select(c => c.First()).ToList();
            }
            return result.ToList();
        }

        public List<CityCodeDto> GetCityCodes(int provinceCode)
        {
            var result = _cityRepository.GetCityCodes(provinceCode);
            return result.Select(q => new CityCodeDto
            {
                Id = q.Id,
                Name = q.Name,
                Parent = q.Parent
            }).ToList();
        }

        public List<Local_V2> GetLocalList(int parent)
        {
            return _cityRepository.GetLocalList(parent);
        }

        public async Task<List<MetroQuery>> GetMetroQuerys(List<Guid> metroLineIds, List<int> metroStationIds)
        {
            metroLineIds = metroLineIds ?? new List<Guid>();
            metroStationIds = metroStationIds ?? new List<int>();

            List<MetroQuery> metroIds = new List<MetroQuery>();
            //if (!metroStationIds.Any())
            //{
            //    return metroIds;
            //}

            if (metroStationIds.Any())
            {
                var metros = await GetMetroList(metroLineIds);

                metroIds = metros.Select(q => new MetroQuery
                {
                    LineId = q.MetroId,
                    StationIds = q.MetroStations.Where(p => metroStationIds.Contains(p.Id))
                                  .Select(p => p.Id).ToList()
                }).ToList();
            }
            else
            {
                metroIds = metroLineIds.Select(q => new MetroQuery
                {
                    LineId = q,
                    StationIds = new List<int>()
                }).ToList();
            }

            return metroIds;
        }
    }
}
