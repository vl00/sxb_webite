using PMS.Infrastructure.Domain;
using PMS.Infrastructure.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using PMS.Infrastructure.Domain.Entities;
using ProductManagement.Framework.MSSQLAccessor;

namespace PMS.Infrastructure.Repository.Repository
{
    public class CityCodeRepository : ICityCodeRepository
    {
        private CityDbContext _dbContext;
        public CityCodeRepository(CityDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<CityCode> GetAllCityCode()
        {
            //暂且保留
            //return _dbContext.Query<CityCode>(@"
            //        select a.id, a.name, a.parentid as parent from[iSchoolData].[dbo].[KeyValue] a
            //        LEFT JOIN[iSchoolData].[dbo].[KeyValue] b on a.parentid = b.id
            //        where a.IsValid = 1 and b.IsValid = 1 and b.parentid = 0 and b.id < 800000", new { }).ToList();
            return _dbContext.Query<CityCode>(@"
                    SELECT A.id, A.name, A.parentid as parent  FROM [iSchoolData].[dbo].[KeyValue] AS A
                    LEFT JOIN [iSchoolData].[dbo].[KeyValue] AS B ON B.id = A.parentid
                    WHERE A.IsValid = 1 AND B.IsValid = 1 AND B.parentid = 0 
                    AND A.id IN(SELECT city FROM dbo.OnlineSchoolExtContent GROUP BY city)", new { }).ToList();
        }

        public List<LocalInfo> GetAeraInfo(List<int> cityCodes)
        {
            return _dbContext.Query<LocalInfo>(
                "select id,name,parentid,type,description from [iSchoolData].[dbo].[KeyValue] where parentid IN @cityCodes order by id asc;;",
                new { cityCodes = cityCodes.ToArray() }
            ).ToList();
        }

        public List<LocalInfo> GetAeraInfo(int cityCode)
        {
            return _dbContext.Query<LocalInfo>(
                "select id,name,parentid,type,description from [iSchoolData].[dbo].[KeyValue] where parentid = @cityCode order by id asc;;",
                new { cityCode }
            ).ToList();
        }

        public LocalInfo GetAeraInfoById(int areaCode)
        {
            return _dbContext.Query<LocalInfo>(
                "select id,name,parentid,type,description from [iSchoolData].[dbo].[KeyValue] where id = @areaCode ",
                new { areaCode }
            ).FirstOrDefault();
        }

        public LocalInfo GetInfoByName(string cityName)
        {
            return _dbContext.Query<LocalInfo>("select id,name,parentid,type,description from [iSchoolData].[dbo].[KeyValue] where name = @cityName and parent <> 0", new { cityName }).FirstOrDefault();
        }

        public LocalInfo GetInfoByCityCode(int CityCode)
        {
            return _dbContext.Query<LocalInfo>("select id,name,parentid,type,description from [iSchoolData].[dbo].[KeyValue] where id = @CityCode", new { CityCode }).FirstOrDefault();
        }

        /// <summary>
        /// 获得所有的省
        /// </summary>
        /// <returns></returns>
        public List<LocalInfo> GetProvinceInfo()
        {
            return _dbContext.Query<LocalInfo>("select id,name,parentid,type,description from [iSchoolData].[dbo].[KeyValue] where  parentid = 0 and id < 800000 order by id asc;", new { }).ToList();
        }
        /// <summary>
        /// 获取城市所在的省份信息
        /// </summary>
        /// <param name="cityCode"></param>
        /// <returns></returns>
        public LocalInfo GetProvinceInfoByCity(int cityCode)
        {
            return _dbContext.Query<LocalInfo>(
                @"select a.id,a.name,a.parentid,a.type,a.description from [iSchoolData].[dbo].[KeyValue] a
                LEFT JOIN [iSchoolData].[dbo].[KeyValue] b on a.id  = b.parentid
                where  a.parentid = 0 and b.id = @cityCode;",
                new { cityCode }
            ).FirstOrDefault();
        }

        public List<AreaPolyline> GetAreaPolyline(int cityCode)
        {
            return _dbContext.Query<AreaPolyline>(
                @"select area.id,area.centerlocation,kv.name 
                from [iSchoolData].[dbo].[area_polyline] area
                inner join [iSchoolData].[dbo].[KeyValue] kv on kv.id = area.id and kv.parentid = @cityCode;",
                new { cityCode }
            ).ToList();
        }

        public List<CityCode> GetCityCodes(int provinceCode)
        {
            return _dbContext.Query<CityCode>("select id,name,parentid as parent from [iSchoolData].[dbo].[KeyValue] where parentid =@provinceCode and IsValid=1 order by id asc;;",
                new { provinceCode }).ToList();
        }

        public List<Local_V2> GetLocalList(int parent)
        {
            return _dbContext.Query<Local_V2>(@"select * from local_v2 where parent=@parent", new { parent }).ToList();
        }
    }
}
