using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.OperationPlateform.Repository
{
    using Domain.IRespositories;
    using Domain.Entitys;
    using ProductManagement.Framework.MSSQLAccessor.DBContext;
    using System.Linq;
    using PMS.OperationPlateform.Domain.DTOs;
    using System.Data;
    using System.Runtime.InteropServices.ComTypes;

    public class AdvertisingBaseRepository : BaseRepository<AdvertisingBase>, IAdvertisingBaseRepository
    {

        public AdvertisingBaseRepository(OperationDBContext db) : base(db)
        {
        }

        public IEnumerable<AdvertisingBase> GetAdvertising(int id)
        {
            string sql = @" SELECT ROW_NUMBER() OVER(ORDER BY AdvertisingScheduleBase_Relationships.Sort) Sort, AdvertisingBase.*  FROM AdvertisingBase 
  JOIN AdvertisingScheduleBase_Relationships ON AdvertisingBase.Id = AdvertisingScheduleBase_Relationships.BaseId
  WHERE 
  AdvertisingBase.IsDelete=0
  AND 
  AdvertisingScheduleBase_Relationships.ScheduleId = (SELECT top 1  AdvertisingSchedule.Id FROM AdvertisingSchedule
  JOIN AdvertisingLocation ON AdvertisingSchedule.[Location] = AdvertisingLocation.Id
  WHERE @DATENOW BETWEEN UpTime AND ExpireTime AND AdvertisingLocation.IsEnable = 1 AND AdvertisingLocation.Id=@LOCATION
  ORDER BY Sort)";
            return _db.Query<AdvertisingBase>(sql, new { DATENOW = DateTime.Now, LOCATION = id });

        }

        /// <summary>
        /// 获取广告及其排序
        /// 排序规则
        /// IsTop 每次查询随机
        /// !IsTop 先城市, 后全国
        /// 最后根据Sort, 创建先后排序
        /// </summary>
        /// <param name="locationId"></param>
        /// <param name="cityIds"></param>
        /// <param name="currentTime"></param>
        /// <returns></returns>
        public IEnumerable<AdvertisingBase> GetAdvertising(int locationId, int[] cityIds, DateTime currentTime, string dataId)
        {
            var dataIds = new[] { "", dataId };
            var dataSql = !string.IsNullOrWhiteSpace(dataId) ? " AND ACSR.DataId in @dataIds " : " AND ACSR.DataType = 0 ";

            var sql = $@"
SELECT
	AB.*,
	ACSR.Sort,
	ACSR.CityId,
	ACSR.DataType,
	ACSR.BeforeCount,
	ACSR.IsTop
FROM
	dbo.Ad_City_Schedule_R ACSR
	INNER JOIN dbo.AdvertisingSchedule S ON S.Id = ACSR.ScheduleId
	INNER JOIN dbo.AdvertisingBase AB on AB.Id = ACSR.AdId AND AB.IsDelete = 0 AND AB.Status = 1
WHERE
	ACSR.IsDeleted = 0
	AND S.Location = @locationId
	AND ACSR.CityId IN @cityIds
    {dataSql}
	AND S.UpTime < @currentTime  -- 已开始的广告
	AND S.ExpireTime > @currentTime  -- 未过期的广告
ORDER BY
	ACSR.IsTop DESC, ACSR.Sort, ACSR.CityId, AB.Id
";
            return _db.Query<AdvertisingBase>(sql, new { locationId, cityIds, currentTime, dataIds });
        }


        public IEnumerable<FixedAdDto> GetFixedAds(int locationId, Guid dataId, DateTime currentTime)
        {
            var sql = $@"
SELECT
	AF.AdId,
    AB.Title,
    AB.Url,
    AB.PicUrl,
    AF.PositionType,
    @dataId as DataId,
    AFD.RefId
FROM
	AdFixed AF
	INNER JOIN dbo.AdvertisingBase AB on AB.Id = AF.AdId AND AB.IsDelete = 0 AND AB.Status = 1
	INNER JOIN dbo.AdvertisingLocation AL ON AL.Id = AF.LocationId
    INNER JOIN dbo.AdFixedData AFD ON AFD.DataId = @dataId AND AFD.FixedId = AF.Id AND AFD.IsDeleted = 0
WHERE
	AF.IsDeleted = 0
	AND AF.LocationId = @locationId
	AND AF.UpTime < @currentTime  -- 已开始的广告
	AND AF.ExpireTime > @currentTime  -- 未过期的广告
";
            return _db.Query<FixedAdDto>(sql, new { locationId, dataId, currentTime });
        }


        public int GetDataCityId(string dataId)
        {
            var sql = $@"
SELECT
	top 1 ACSR.CityId
FROM
	iSchoolArticle.dbo.Ad_City_Schedule_R ACSR
WHERE
	ACSR.DataId = dataId
";
            return _db.Query<int>(sql, new { dataId }).FirstOrDefault();
        }
    }
}
