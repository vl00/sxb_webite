using PMS.Infrastructure.Application.ModelDto;
using PMS.Infrastructure.Domain;
using PMS.Infrastructure.Domain.Entities;
using PMS.Search.Application.ModelDto.Query;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PMS.Infrastructure.Application.IService
{
    public interface ICityInfoService
    {
        List<CityCode> GetAllCityCode();
        Task<List<CityCode>> GetAllCityCodes();
        Task<List<AdCodeDto>> IntiAdCodes();
        Task<List<AreaDto>> GetAreaCode(string cityName);

        Task<List<CityAreaDto>> GetAreaCode(List<int> cityCodes);
        Task<List<AreaDto>> GetAreaCode(int cityCode);
        Task<List<MetroDto>> GetMetroList(int cityCode);
        Task<List<MetroDto>> GetMetroList(List<Guid> metroLineIds);

        Task<List<AreaPolylineDto>> GetAreaPolyline(int cityCode);

        Task<AreaDto> GetInfoByCityName(string cityName);
        Task<AreaDto> GetInfoByCityCode(int CityCode);
        Task<List<AreaDto>> GetHotCity();

        Task<Dictionary<char, List<CitySearchDto>>> GetAllCityPinYin();

        /// <summary>
        /// 获取所有省份代码和省份名称
        /// </summary>
        /// <returns></returns>
        Task<List<ProvinceCodeDto>> GetProvince();
        /// <summary>
        /// 获取所有省份代码和省份名称以及省份下的城市
        /// </summary>
        /// <returns></returns>
        Task<List<ProvinceCodeDto>> GetProvinceInfos();
        /// <summary>
        /// 获取指定省份的代码和名称以及该省份下的城市
        /// </summary>
        /// <returns></returns>
        Task<ProvinceCodeDto> GetProvinceInfo(int ProvinceCode);
        /// <summary>
        /// 获取指定城市所在省份代码和省份名称以及省份下的城市
        /// </summary>
        /// <returns></returns>
        Task<ProvinceCodeDto> GetProvinceInfoByCity(int CityCode);

        /// <summary>
        /// 根据citycode获取城市名
        /// </summary>
        Task<string> GetCityName(int city);
        List<CityCodeDto> GetCityCodes(int provinceCode);

        List<Local_V2> GetLocalList(int parent);
        Task<List<MetroQuery>> GetMetroQuerys(List<Guid> metroLineIds, List<int> metroStationIds);
        Task<int> GetCityCodeByAreaCode(int areaCode);
    }
}
