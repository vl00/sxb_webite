using System;
using System.Collections.Generic;
using System.Linq;
using PMS.Infrastructure.Domain.Entities;
using PMS.Infrastructure.Domain.IRepositories;

namespace PMS.Infrastructure.Repository.Repository
{
    public class MetroInfoRepository: IMetroInfoRepository
    {
        private JcDbContext _dbContext;
        public MetroInfoRepository(JcDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<MetroInfo> GetMetroInfoList(int cityCode)
        {
            var str_SQL = @"SELECT
	                            mi.id AS MetroId,
	                            mi.name AS MetroName,
	                            mli.id AS MetroLineId,
	                            mli.name AS MetroLineName,
	                            mli.latitude,
	                            mli.longitude 
                            FROM
	                            [iSchoolData].[dbo].metro_info mi
	                            INNER JOIN [iSchoolData].[dbo].metro_line_info mli ON mli.metro_info_id = mi.id 
                            WHERE
	                            mi.location_id = @cityCode 
                            ORDER BY
	                            mi.location_id ,
	                            mli.id";
            return _dbContext.Query<MetroInfo>(str_SQL,
                new { cityCode }
            ).ToList();
        }
        public List<MetroInfo> GetMetroInfoList(List<Guid> metroLineIds)
        {
            string sql = @"SELECT mi.id as MetroId , mi.name as MetroName,
                        mli.id as MetroLineId ,mli.name as MetroLineName,mli.latitude,mli.longitude
                        FROM [iSchoolData].[dbo].metro_info mi
                        inner join [iSchoolData].[dbo].metro_line_info mli on mli.metro_info_id = mi.id
                        where mi.id IN @metroLineIds;";
            return _dbContext.Query<MetroInfo>(sql,
                new { metroLineIds = metroLineIds.ToArray() }
            ).ToList();
        }
    }
}
