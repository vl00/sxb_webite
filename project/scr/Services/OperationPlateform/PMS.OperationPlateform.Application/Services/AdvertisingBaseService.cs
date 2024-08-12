using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.OperationPlateform.Application.Services
{
    using IServices;
    using Domain.IRespositories;
    using PMS.OperationPlateform.Domain.Entitys;
    using PMS.OperationPlateform.Application.Dtos;
    using System.Linq;
    using PMS.OperationPlateform.Domain.Enums;
    using PMS.OperationPlateform.Domain.DTOs;

    public class AdvertisingBaseService : IAdvertisingBaseService
    {
        IAdvertisingBaseRepository _repository;

        public AdvertisingBaseService(IAdvertisingBaseRepository repository)
        {
            this._repository = repository;
        }

        /// <summary>
        /// 全国城市Id
        /// </summary>
        public static int AllCityId = 0;

        public IEnumerable<AdvertisingBaseGetAdvertisingResultDto> GetAdvertising(int locationId, int cityId, string dataId = null)
        {
            DateTime currentTime = DateTime.Now;
            var cityIds = new[] { cityId, AllCityId };
            if (!string.IsNullOrWhiteSpace(dataId))
            {
                var dataCityId = _repository.GetDataCityId(dataId);
                cityIds = new[] { cityId, AllCityId, dataCityId };
            }

            var ads = _repository.GetAdvertising(locationId, cityIds, currentTime, dataId).ToList();
            ads = SortAds(ads);
            var bodyAds = ads.Select(s =>
            {
                var dto = (AdvertisingBaseGetAdvertisingResultDto)s;
                dto.Url = AdvertisingBaseGetAdvertisingResultDto.MakeUrl(dto.Url, dataId);
                return dto;
            });

            if (dataId != null && Guid.TryParse(dataId, out Guid id))
            {
                var _fixedAds = _repository.GetFixedAds(locationId, id, currentTime);
                return ConcatFixedAd(ads, bodyAds, _fixedAds);
            }
            else
            {
                return bodyAds;
            }

        }

        private static IEnumerable<AdvertisingBaseGetAdvertisingResultDto> ConcatFixedAd(List<AdvertisingBase> ads, IEnumerable<AdvertisingBaseGetAdvertisingResultDto> bodyAds, IEnumerable<FixedAdDto> _fixedAds)
        {
            var fixedAds = _fixedAds.Where(s => !ads.Any(ad => ad.Id == s.AdId));
            //前缀固定
            var prefixAdsDto = fixedAds.Where(s => s.PositionType == 1).Select(s => (AdvertisingBaseGetAdvertisingResultDto)s);
            //后缀固定
            var subfixAdsDto = fixedAds.Where(s => s.PositionType == 2).Select(s => (AdvertisingBaseGetAdvertisingResultDto)s);
            return prefixAdsDto.Concat(bodyAds).Concat(subfixAdsDto);
        }


        /// <summary>
        /// 广告位广告排期详情
        /// </summary>
        /// <param name="locationId"></param>
        /// <param name="cityId"></param>
        /// <param name="currentTime"></param>
        /// <returns></returns>
        public List<AdvertisingBase> SortAds(List<AdvertisingBase> ads)
        {
            var unTopAds = ads.Where(s => !s.IsTop);
            var topAds = ads.Where(s => s.IsTop);

            var cityAds = unTopAds.Where(s => s.DataType == LocationDataType.Default).ToList();
            var dataAds = unTopAds.Where(s => s.DataType != LocationDataType.Default).ToList();

            var newAds = GetNewAds(dataAds, cityAds);

            newAds = topAds.Concat(newAds).ToList();
            RandTopAd(ref newAds);
            return newAds;
        }

        public List<AdvertisingBase> GetNewAds(List<AdvertisingBase> currentAds, List<AdvertisingBase> parentAds)
        {
            var newAds = new List<AdvertisingBase>();
            var skip = 0;
            foreach (var ad in currentAds)
            {
                for (int i = 0; i < ad.BeforeCount; i++)
                {
                    var cityAd = parentAds.Skip(skip++).FirstOrDefault();
                    if (cityAd != null)
                    {
                        newAds.Add(cityAd);
                    }
                }
                newAds.Add(ad);
            }

            if (skip <= parentAds.Count - 1)
            {
                newAds = newAds.Concat(parentAds.Skip(skip)).ToList();
            }
            return newAds;
        }

        public void RandTopAd(ref List<AdvertisingBase> ads)
        {
            if (ads == null || ads.Count == 0)
            {
                return;
            }

            var tops = ads.Where(s => s.IsTop).ToList();
            var topsCount = tops.Count;
            var random = new Random();
            for (int i = 0; i < topsCount; i++)
            {
                //rand一个数, 放到第一位
                var rand = random.Next(i, topsCount);
                if (i != rand)
                {
                    var temp = ads[i];
                    ads[i] = ads[rand];
                    ads[rand] = temp;
                }
            }
        }

        public IEnumerable<AdvertisingBase> GetAdvertisings(int[] ids)
        {
            return this._repository.Select("id in @ids ", new { ids });
        }
    }
}
