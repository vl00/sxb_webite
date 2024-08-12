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
    public class ToFenghuangtongRepository : IToFenghuangtongRepository
    {
        private readonly ISchoolDataDBContext dbc;

        public ToFenghuangtongRepository(ISchoolDataDBContext dbc)
        {
            this.dbc = dbc;
        }
        
        public Guid GetSchoolEid(long no)
        {
            var sql = "select id From OnlineSchoolExtension where [No]=@no ;";
            return dbc.Query<Guid>(sql, new { no }).FirstOrDefault();
        }

        public async Task<SchExtDto1> GetSchExtDto1(Guid eid)
        {
            var no = dbc.QuerySingle<long>($"select no from onlineschoolextension where id = @eid", new { eid });

            var sql = $@"
select e.eid,e.sid,e.SchName,e.Extname,e.Latitude,e.Longitude
from Lyega_OLschextSimpleInfo e 
where e.eid=@eid
";
            var dto = (await dbc.QueryAsync<SchExtDto1>(sql, new { eid })).FirstOrDefault();
            if (dto != null)
            {
                dto._Eno = no;
                return dto;
            }

            sql = $@"
select top 1 e.eid,e.sid,e.SchName,e.Extname,e.Latitude,e.Longitude
from Lyega_OLschextSimpleInfo e 
inner join Lyega_ExtId_OldToNew o2n on e.eid=o2n.NewId
where o2n.OldId=@eid
";
            dto = (await dbc.QueryAsync<SchExtDto1>(sql, new { eid })).FirstOrDefault();
            if (dto != null)
            {
                dto._Eno = no;
                return dto;
            }

            return null;
        }
    }
}