using PMS.OperationPlateform.Domain.DTOs;
using PMS.OperationPlateform.Domain.Entitys;
using System;
using System.Collections.Generic;
using System.Security.Policy;
using System.Text;

namespace PMS.OperationPlateform.Application.Dtos
{
    public class AdvertisingBaseGetAdvertisingResultDto
    {
        public int Id { get; set; }

        public string SloGan { get; set; }

        public string Url { get; set; }

        public string PicUrl { get; set; }

        public int Sort { get; set; }

        public double Rate { get; set; }

        public static implicit operator AdvertisingBaseGetAdvertisingResultDto(AdvertisingBase advertisingBase)
        {
            return new AdvertisingBaseGetAdvertisingResultDto()
            {
                Id = advertisingBase.Id,
                PicUrl = advertisingBase.PicUrl,
                Sort = advertisingBase.Sort,
                SloGan = advertisingBase.Title,
                Url = advertisingBase.Url
            };
        }

        public static implicit operator AdvertisingBaseGetAdvertisingResultDto(FixedAdDto fixedAd)
        {
            return new AdvertisingBaseGetAdvertisingResultDto()
            {
                Id = fixedAd.AdId,
                PicUrl = fixedAd.PicUrl,
                Sort = fixedAd.PositionType == 1 ? -9999 : 9999,
                SloGan = fixedAd.Title,
                Url = MakeUrl(fixedAd)
            };
        }

        public static string MakeUrl(FixedAdDto fixedAd)
        {
            var url = fixedAd.Url;
            if (string.IsNullOrWhiteSpace(url))
            {
                return url;
            }
            if (fixedAd.DataId != null)
            {
                url = url.Replace("{ad_dataId}", fixedAd.DataId.ToString());
            }
            if (fixedAd.RefId != null)
            {
                url = url.Replace("{ad_refId}", fixedAd.RefId.ToString());
            }
            return url;
        }

        public static string MakeUrl(string originUrl, string dataId)
        {
            if (!string.IsNullOrWhiteSpace(dataId) && !string.IsNullOrWhiteSpace(originUrl))
            {
                return originUrl
                     .Replace("{ad_dataId}", dataId.ToString())
                     ;
            }
            return originUrl;
        }
    }
}
