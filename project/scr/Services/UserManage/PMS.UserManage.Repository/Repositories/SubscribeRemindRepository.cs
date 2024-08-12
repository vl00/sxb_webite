using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using PMS.UserManage.Domain.Common;
using PMS.UserManage.Domain.Dtos;
using PMS.UserManage.Domain.Entities;
using PMS.UserManage.Domain.IRepositories;
using static PMS.UserManage.Domain.Common.EnumSet;

namespace PMS.UserManage.Repository.Repositories
{
    public class SubscribeRemindRepository : ISubscribeRemindRepository
    {
        private readonly UserDbContext _dbcontext;
        public SubscribeRemindRepository(UserDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }


        public bool Add(SubscribeRemind subscribeRemind)
        {
            return _dbcontext.Add<SubscribeRemind>(subscribeRemind) != null;
        }

        public bool Update(SubscribeRemind subscribeRemind)
        {
            return _dbcontext.Update(subscribeRemind);
        }

        public bool Exists(string groupCode, Guid subjectId, Guid userId)
        {
            var sql = @"
select 
    isnull(count(1), 0) as Total
from
    SubscribeRemind
where 
    IsValid = 1
    and groupCode = @groupCode
    and subjectId = @subjectId
    and userId = @userId
";
            return _dbcontext.QuerySingle<int>(sql, new { groupCode, subjectId, userId }) > 0;
        }

        public IEnumerable<SubscribeRemind> GetPagination(string groupCode, int pageIndex = 1, int pageSize = 10)
        {
            var sql = @"
select
    *
from
    SubscribeRemind
where
    IsValid = 1
    and groupCode = @groupCode
order by time desc
offset (@pageIndex-1)*@pageSize rows 
fetch next @pageSize row only ";
            return _dbcontext.Query<SubscribeRemind>(sql, new { groupCode, pageIndex, pageSize });
        }

    }
}
