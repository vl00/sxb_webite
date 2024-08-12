using iSchool;
using Microsoft.Extensions.Configuration;
using PMS.CommentsManage.Domain.Common;
using PMS.School.Domain.Common;
using PMS.School.Domain.Dtos;
using PMS.School.Domain.Entities;
using PMS.School.Domain.IRepositories;
using ProductManagement.Framework.EntityFramework;
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
    public class HotPopularRepository : IHotPopularRepository
    {
        ISchoolDataDBContext dbc;
        ISchRepository schRepository;
        string iSchoolUser_dbo;

        public HotPopularRepository(ISchoolDataDBContext dbc, IConfiguration configuration, ISchRepository schRepository)
        {
            this.dbc = dbc;
            this.iSchoolUser_dbo = configuration["crossDBprex:iSchoolUser_dbo"];
            this.schRepository = schRepository;
        }

        public SimpleHotSchoolDto[] GetHotVisitSchools(int citycode, SchFType0? schtype, int count = 6)
        {
            var sql = $@"
drop table IF EXISTS #uh;

select uh.dataID as eid,count(uh.userID)as usercount into #uh from {iSchoolUser_dbo}.history uh 
where uh.datatype=1 and (DATEDIFF(dd,uh.time,@now) between 0 and 2)
group by uh.dataID

select top {count} * from(select e.eid,e.sid,(e.schname+'-'+e.extname)as [name],e.lodging,e.sdextern,e.type as SchoolType,
e.SchFType,uh.usercount,(case when @citycode<=0 then 1 when e.city=@citycode then 1 else 0 end)as _c,osc.[No] as [SchoolNo]
from dbo.Lyega_OLschextSimpleInfo e 
left join #uh uh on uh.eid=e.eid
leFT JOiN OnlineSchoolExtension as osc on osc.id = e.eid
where 1=1
)T where 1=1 {"and SchFType=@schtype".If(!schtype.In(null, default(SchFType0)))}
order by _c desc,usercount desc

";
            var dtos = dbc.Query<SimpleHotSchoolDto>(sql, new { citycode, schtype = schtype?.ToString(), now = DateTime.Now }).ToArray();

            var scores = schRepository.GetSchoolsTotalScores(dtos.Select(_ => _.Eid).ToArray());

            scores.Aggregate(dtos, (__, score) =>
            {
                var dto = dtos.FirstOrDefault(_ => _.Eid == score.Eid);
                dto.TotalScore = score.Score;
                return dtos;
            });

            return dtos;
        }

        public SmpNearestSchoolDto[] GetNearestSchools(Guid eid, int count = 6)
        {
            var sql = $@"
declare @province int;
declare @city int;
declare @lnglat geography;
declare @schtype varchar(50);

select @lnglat=c.LatLong,@province=c.province,@city=c.city,@schtype=c.SchFType
from Lyega_OLschextSimpleInfo c 
where c.eid=@eid 

select top {count} * from(select e.eid,e.sid,(e.schname+'-'+e.extname)as [name],e.lodging,e.sdextern, --sc.score as totalscore,
e.SchFType,(case when e.LatLong is null then -1 else abs(e.LatLong.STDistance(@lnglat)) end)as Distance,
(case when e.city=@city then 1 else 2 end)as _order,e.IsAuthedByOpen
from Lyega_OLschextSimpleInfo e 
--left join [Score] sc on sc.eid=e.eid and sc.indexid=22
where 1=1
and e.eid<>@eid --and e.city=@city
)T where Distance>0 --and SchFType=@schtype
order by _order,Distance
"; 
            var dtos = dbc.Query<SmpNearestSchoolDto>(sql, new { eid }).ToArray();

            var scores = schRepository.GetSchoolsTotalScores(dtos.Select(_ => _.Eid).ToArray());

            scores.Aggregate(dtos, (__, score) =>
            {
                var dto = dtos.FirstOrDefault(_ => _.Eid == score.Eid);
                dto.TotalScore = score.Score;
                return dtos;
            });

            return dtos;
        }

        public SmpNearestSchoolDto[] GetNearestSchools(int citycode, (double Lng, double Lat) lnglat, SchFType0[] schtypes, int count = 6)
        {
            var sql = $@"
declare @lnglat geography = geography::STPointFromText('POINT({lnglat.Lng} {lnglat.Lat})',4326) ;

select top {count} * from(select e.eid,e.sid,(e.schname+'-'+e.extname)as [name],e.lodging,e.sdextern, ----sc.score as totalscore,
e.SchFType,(case when e.LatLong is null then -1 else abs(e.LatLong.STDistance(@lnglat)) end)as Distance,
(case when e.city=@city then 1 else 2 end)as _order,e.IsAuthedByOpen
from Lyega_OLschextSimpleInfo e 
----left join [Score] sc on sc.eid=e.eid and sc.indexid=22
--where 1=1
)T where Distance>0 {"and SchFType in @schtypes".If((schtypes?.Length ?? 0) > 0)}
order by _order,Distance
";
            var dtos = dbc.Query<SmpNearestSchoolDto>(sql, new { city = citycode, schtypes = (schtypes ?? new SchFType0[0]).Select(_ => _.ToString()) }).ToArray();

            var scores = schRepository.GetSchoolsTotalScores(dtos.Select(_ => _.Eid).ToArray());

            scores.Aggregate(dtos, (__, score) =>
            {
                var dto = dtos.FirstOrDefault(_ => _.Eid == score.Eid);
                dto.TotalScore = score.Score;
                return dtos;
            });

            return dtos;
        }

        public (Guid Sid, Guid Eid)[] GetHotVisitSchextIds(int citycode, byte? grade, byte? type, int day = 7, int count = 6)
        {
            var sql = $@"
select uh.dataID as eid,count(uh.userID)as uv into #uh from {iSchoolUser_dbo}.history uh 
where uh.datatype=1 and (DATEDIFF(dd,uh.time,@now) between 0 and {day})
group by uh.dataID

select top {count} * from(select e.eid,e.sid,(e.schname+'-'+e.extname)as [name],--c.lodging,e.sdextern,sc.score as totalscore,
e.SchFType,uh.uv,(case when @citycode<=0 then 1 when e.city=@citycode then 1 else 0 end)as _c
from dbo.Lyega_OLschextSimpleInfo e 
left join #uh uh on uh.eid=e.eid
where 1=1 {"and e.grade=@grade".If(grade != null)} {"and e.type=@type".If(type != null)}
)T order by _c desc,uv desc

drop table #uh
";
            var dtos = dbc.Query<SimpleHotSchoolDto>(sql, new { grade, type, citycode, now = DateTime.Now });
            return dtos.Select(_ => (_.Sid, _.Eid)).ToArray();
        }
    }
}

