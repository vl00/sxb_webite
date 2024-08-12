using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.OperationPlateform.Application.IServices
{
    using Domain.Entitys;
    using PMS.OperationPlateform.Application.Dtos;

    public interface IAdvertisingBaseService
    {
        /// <summary>
        /// 获取指定广告位的广告列表
        /// <para>modified by Labbor on 20121215 添加城市查询, Random IsTop</para>
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        IEnumerable<AdvertisingBaseGetAdvertisingResultDto> GetAdvertising(int locationId, int cityId, string dataId = null);


        [Obsolete("该方法已弃用",true)]
        IEnumerable<AdvertisingBase> GetAdvertisings(int[] ids);


    }
}
