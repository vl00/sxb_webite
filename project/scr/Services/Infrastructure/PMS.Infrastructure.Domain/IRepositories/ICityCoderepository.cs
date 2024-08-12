using System;
using System.Collections.Generic;
using System.Text;
using PMS.Infrastructure.Domain.Entities;

namespace PMS.Infrastructure.Domain.IRepositories
{
    public interface ICityCodeRepository
    {
        List<CityCode> GetAllCityCode();
        /// <summary>
        /// 获得所有的省份信息
        /// </summary>
        /// <returns></returns>
        List<LocalInfo> GetProvinceInfo();
        LocalInfo GetProvinceInfoByCity(int cityCode);


        List<LocalInfo> GetAeraInfo(List<int> cityCodes);

        List<LocalInfo> GetAeraInfo(int cityCode);
        LocalInfo GetInfoByName(string cityName);

        List<AreaPolyline> GetAreaPolyline(int cityCode);

        LocalInfo GetInfoByCityCode(int CityCode);
        List<CityCode> GetCityCodes(int provinceCode);

        List<Local_V2> GetLocalList(int parent);
        LocalInfo GetAeraInfoById(int areaCode);
    }
}
