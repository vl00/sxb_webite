using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.OperationPlateform.Domain.IRespositories
{
    using Entitys;
    using PMS.OperationPlateform.Domain.DTOs;

    public interface IAdvertisingBaseRepository:IBaseRepository<AdvertisingBase>
    {
        IEnumerable<AdvertisingBase> GetAdvertising(int id);
        IEnumerable<AdvertisingBase> GetAdvertising(int locationId, int[] cityIds, DateTime currentTime, string dataId);
        int GetDataCityId(string dataId);
        IEnumerable<FixedAdDto> GetFixedAds(int locationId, Guid dataId, DateTime currentTime);
    }
}
