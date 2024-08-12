using PMS.UserManage.Domain.Entities;
using PMS.UserManage.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Text;
using static PMS.UserManage.Repository.UserDbContext;

namespace PMS.UserManage.Repository.Repositories
{
    public class TopicCircleRepository : ITopicCircleRepository
    {
        private readonly UserDbContext _dbcontext;
        public TopicCircleRepository(UserDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public bool AddOrUpdateCircleFollower(IEnumerable<CircleFollower> circleFollowers)
        {
            var sql = $@"
                        merge into {DbNameHelper.ISchoolTopicCircle}.CircleFollower T1
                        using(
                            select @UserId as UserId, @CircleId as CircleId
                        ) T2 on T2.UserId = T1.UserId and T2.CircleId = T1.CircleId
                        when matched then update
                            set ModifyTime = @ModifyTime
                        when not matched then insert
                            (Id, UserId, CircleId, Time, ModifyTime) values (@Id, @UserId, @CircleId, @Time, @ModifyTime)

;
                        ";
            return _dbcontext.ExecuteUow(sql, circleFollowers) > 0;
        }

        public IEnumerable<Guid> GetCircle(Guid creator)
        {
            var sql = $@" select Id from {DbNameHelper.ISchoolTopicCircle}.Circle where UserId  = @userId ";
            return _dbcontext.Query<Guid>(sql, new { userId = creator });

        }
    }
}
