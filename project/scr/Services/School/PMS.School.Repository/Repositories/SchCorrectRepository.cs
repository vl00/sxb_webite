using PMS.School.Domain.Common;
using PMS.School.Domain.Dtos;
using PMS.School.Domain.Entities;
using PMS.School.Domain.IRepositories;
using ProductManagement.Framework.MSSQLAccessor.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMS.School.Repository.Repositories
{
    public class SchCorrectRespository : ISchCorrectRepository
    {
        ISchoolDataDBContext dbc;

        public SchCorrectRespository(ISchoolDataDBContext dbc)
        {
            this.dbc = dbc;
        }

        public SchSourceInfoDto GetSchSourceInfo(Guid eid)
        {
            var sql = $@"
select e.id as eid,e.sid,s.name as sname,e.name as ename,(CONVERT(varchar(10),e.grade)+'.'+CONVERT(varchar(10),e.type)+'.'+CONVERT(varchar(10),isnull(e.discount,0))+'.'+CONVERT(varchar(10),isnull(e.diglossia,0))+'.'+CONVERT(varchar(10),isnull(e.chinese,0)))as SchType,
c.address,c.latitude as lat,c.longitude as lng
from dbo.OnlineSchoolExtension e
inner join dbo.OnlineSchool s on e.sid=s.id
left join dbo.OnlineSchoolExtContent c on c.eid=e.id and c.IsValid=1
where e.IsValid=1 and s.IsValid=1 and s.status={(int)SchoolStatus.Success}
and e.id=@eid
";
            var dto = dbc.Query<SchSourceInfoDto>(sql, new { eid }).FirstOrDefault();
            return dto;            
        }

        public bool Insert(ExtraSchCorrect0 correct)
        {
            var i = dbc.Insert(correct);
            return i > 0;
        }
    }
}
