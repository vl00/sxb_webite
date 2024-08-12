using iSchool;
using PMS.CommentsManage.Domain.Common;
using PMS.School.Domain.Common;
using PMS.School.Domain.Dtos;
using PMS.School.Domain.Entities;
using PMS.School.Domain.IRepositories;
using ProductManagement.Framework.EntityFramework;
using ProductManagement.Framework.Foundation;
using ProductManagement.Framework.MSSQLAccessor.DBContext;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMS.School.Repository.Repositories
{
    public class SchRepository : ISchRepository
    {
        readonly ISchoolDataDBContext dbc;

        public SchRepository(ISchoolDataDBContext dbc)
        {
            this.dbc = dbc;
        }

        public SchExtDto0 GetSchextSimpleInfo(Guid eid)
        {
            var schoolNo = dbc.QuerySingle<int>($"select no from onlineschoolextension where id = @eid", new { eid });

            var sql = $@"
select e.eid,e.sid,e.SchName,e.Extname,e.grade,e.type,e.discount,e.diglossia,e.chinese,e.province,e.city,e.area
from Lyega_OLschextSimpleInfo e 
where e.eid=@eid
";
            var dto = dbc.Query<SchExtDto0>(sql, new { eid }).FirstOrDefault();
            if (dto != null) { dto.SchoolNo = schoolNo; return dto; }

            sql = $@"
select top 1 e.eid,e.sid,e.SchName,e.Extname,e.grade,e.type,e.discount,e.diglossia,e.chinese,e.province,e.city,e.area
from Lyega_OLschextSimpleInfo e 
inner join Lyega_ExtId_OldToNew o2n on e.eid=o2n.NewId
where o2n.OldId=@eid
";
            dto = dbc.Query<SchExtDto0>(sql, new { eid }).FirstOrDefault();
            if (dto != null) { dto.SchoolNo = schoolNo; return dto; }

            return dto;
        }

        public SchExtDto0[] GetNearSchoolsBySchType((double Lng, double Lat) location, SchFType0[] schFTypes, int count = 8)
        {
            var sql = $@"
declare @lnglat geography; set @lnglat=geography::STPointFromText('POINT({location.Lng} {location.Lat})', 4326);

-- select top {count} e.eid,e.sid,e.SchName,e.Extname,e.grade,e.type,e.discount,e.diglossia,e.chinese,e.SchFType,e.province,e.city,e.area,
--  abs(e.LatLong.STDistance(@lnglat)) as Distance
-- from Lyega_OLschextSimpleInfo e  
-- where e.LatLong is not null {"and SchFType in @schFTypes".If((schFTypes?.Length ?? 0) > 0)}
-- order by Distance 

            SELECT TOP {count}
	            e.eid,
	            e.sid,
	            e.SchName,
	            e.Extname,
	            e.grade,
	            e.type,
	            e.discount,
	            e.diglossia,
	            e.chinese,
	            e.SchFType as [SchFType0],
	            e.province,
	            e.city,
	            e.area,
	            e.Lodging,
	            e.Sdextern,
	            ose.[No] AS [SchoolNo],
	            abs( e.LatLong.STDistance ( @lnglat ) ) AS Distance 
            FROM
	            Lyega_OLschextSimpleInfo AS e
	            LEFT JOIN OnlineSchoolExtension AS ose ON ose.id = e.eid
            WHERE
	            e.LatLong IS NOT NULL
                {"and SchFType in @schFTypes".If((schFTypes?.Length ?? 0) > 0)}
            ORDER BY
	            Distance;";

            var dtos = dbc.Query<SchExtDto0>(sql, new { schFTypes = (schFTypes ?? new SchFType0[0]).Select(_ => _.ToString()) }).ToArray();
            return dtos;
        }

        public async Task<IEnumerable<SchExtDto0>> GetNearSchoolsByEID(Guid eid, int count = 8)
        {
            var str_SQL = $@"
            declare @lnglat geography;
            declare @schtype varchar(50);

            SELECT
	            @lnglat = c.LatLong,@schtype = c.SchFType 
            FROM
	            Lyega_OLschextSimpleInfo c 
            WHERE
	            c.eid=@eid;

            SELECT TOP {count}
	            e.eid,
	            e.sid,
	            e.SchName,
	            e.Extname,
	            e.grade,
	            e.type,
	            e.discount,
	            e.diglossia,
	            e.chinese,
	            e.SchFType as [SchFType0],
	            e.province,
	            e.city,
	            e.area,
	            e.Lodging,
	            e.Sdextern,
	            ose.[No] AS [SchoolNo],
	            abs( e.LatLong.STDistance ( @lnglat ) ) AS Distance 
            FROM
	            Lyega_OLschextSimpleInfo AS e
	            LEFT JOIN OnlineSchoolExtension AS ose ON ose.id = e.eid
            WHERE
	            e.LatLong IS NOT NULL 
	            AND e.SchFType = @schtype
                AND e.eid != @eid
            ORDER BY
	            Distance;";

            var finds = await dbc.QueryAsync<SchExtDto0>(str_SQL, new { eid });

            var scores = GetSchoolsTotalScores(finds.Select(_ => _.Eid).ToArray());

            scores.Aggregate(finds, (__, score) =>
            {
                var dto = finds.FirstOrDefault(_ => _.Eid == score.Eid);
                dto.TotalScore = score.Score;
                return finds;
            });

            return finds;
        }

        public (Guid, string)[] GetLocalColleges()
        {
            var sql = "select id as item1,name as item2 from College where IsValid=1 and type=1";
            return dbc.Query<(Guid, string)>(sql, new object()).ToArray();
        }

        public SchoolExtScore[] GetSchoolsTotalScores(Guid[] eids)
        {
            var sql = "select * from score where indexid=22 and eid in @eids";
            return dbc.Query<SchoolExtScore>(sql, new { eids }).ToArray();
        }

        public Guid GetSchoolextID(string shortSchoolNo)
        {
            var str_SQL = "Select ID From OnlineSchoolExtension WHERE No = @shortSchoolNo;";
            return dbc.QuerySingle<Guid>(str_SQL, new { shortSchoolNo = UrlShortIdUtil.Base322Long(shortSchoolNo) });
        }

        public (Guid, int)[] GetSchoolextNo(Guid[] eids)
        {
            if (eids == null || eids.Length < 1)
            {
                return null;
            }
            var str_SQL = "Select id as item1,[No] as item2 From OnlineSchoolExtension WHERE id in @eids";
            return dbc.Query<(Guid, int)>(str_SQL, new { eids }).ToArray();
        }
    }
}