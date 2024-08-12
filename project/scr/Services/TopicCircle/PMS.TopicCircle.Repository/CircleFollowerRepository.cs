using PMS.TopicCircle.Domain.Dtos;
using PMS.TopicCircle.Domain.Entities;
using PMS.TopicCircle.Domain.Repositories;
using ProductManagement.Framework.MSSQLAccessor.DBContext;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Threading.Tasks;

namespace PMS.TopicCircle.Repository
{
    public class CircleFollowerRepository : Repository<CircleFollower, TopicCircleDBContext>, ICircleFollowerRepository
    {
        TopicCircleDBContext _dbContext;
        public CircleFollowerRepository(TopicCircleDBContext dBContext) : base(dBContext)
        {
            _dbContext = dBContext;
        }

        public async Task<IEnumerable<KeyValuePair<Guid, bool>>> CheckIsFollowCircle(IEnumerable<Guid> circleIDs, Guid userID)
        {
            if (circleIDs == null) return new Dictionary<Guid, bool>();
            var str_SQL = $@"SELECT
	                            CircleId 
                            FROM
	                            CircleFollower 
                            WHERE
	                            UserId = @userID 
	                            AND CircleId IN @circleIDs";
            var finds = await _dbContext.QueryAsync<Guid>(str_SQL, new { circleIDs, userID }) ?? new List<Guid>();
            return circleIDs.Select(p => new KeyValuePair<Guid, bool>(p, finds.Any(x => x == p)));
        }

        public bool Delete(Guid userId, Guid circleId)
        {
            string sql = @"DELETE CircleFollower WHERE USERID = @userId AND CIRCLEID = @circleId";
            return _dbContext.Execute(sql, new { userId, circleId }) > 0;
        }

        public IEnumerable<CircleFollower> ExcuteUSP_QUERYCIRCLEFOLLOWERDETAIL(Guid circleId)
        {
            return _dbContext.Query<CircleFollower>("USP_QUERYCIRCLEFOLLOWERDETAIL", new { circleId }, commandType: System.Data.CommandType.StoredProcedure);
        }

        public IEnumerable<CircleFollower> ExcuteUSP_SEARCHCIRCLEFOLLOWER(Guid circleId, string like)
        {
            return _dbContext.Query<CircleFollower>("USP_SEARCHCIRCLEFOLLOWER", new { circleId, like = $"%{like}%" }, commandType: System.Data.CommandType.StoredProcedure);
        }

        public IEnumerable<CircleFollowerCountDto> GetFollowerCount(params Guid[] circleId)
        {
            string sql = @"SELECT CircleId, COUNT(1) followerCount FROM CircleFollower 
                            WHERE CircleId IN @circleIds
                            GROUP BY CircleId ";
            return _dbContext.Query<CircleFollowerCountDto>(sql, new { circleIds = circleId });

        }

        public int GetNewFollowers(Guid circleId, DateTime? time)
        {
            time = time == null ? SqlDateTime.MinValue.Value : time;
            string sql = @"SELECT COUNT(1) FROM CircleFollower WHERE 
                CircleId=@circleId
                AND
                [Time]>@time
                ";
            return this._dbContext.ExecuteScalar<int>(sql, new { circleId, time });
        }
    }
}
