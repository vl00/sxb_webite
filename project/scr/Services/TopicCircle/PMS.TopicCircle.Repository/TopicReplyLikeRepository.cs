using PMS.TopicCircle.Domain.Entities;
using PMS.TopicCircle.Domain.Repositories;
using ProductManagement.Framework.MSSQLAccessor.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static PMS.TopicCircle.Domain.Enum.GlobalEnum;

namespace PMS.TopicCircle.Repository
{
    public class TopicReplyLikeRepository : Repository<TopicReplyLike, TopicCircleDBContext>, ITopicReplyLikeRepository
    {
        TopicCircleDBContext _dbContext;
        public TopicReplyLikeRepository(TopicCircleDBContext dbContext) : base(dbContext)
        {
            this._dbContext = dbContext;
        }

        public TopicReplyLike GetUow(Guid userId, Guid topicReplyId)
        {
            var sql = $@" select * from TopicReplyLike where  UserId = @userId and TopicReplyId = @topicReplyId";
            return _dbContext.QueryUow<TopicReplyLike>(sql, new { userId, topicReplyId }).FirstOrDefault();
        }

        public int Add(TopicReplyLike like)
        {
            //            var sql = $@" 
            //Merge into TopicReplyLike T1
            //using( 
            //    select @UserId as UserId, @TopicReplyId as TopicReplyId 
            //) T2
            //on(t1.UserId = T2.UserId and T1.TopicReplyId = T2.TopicReplyId)
            //when matched then update 
            //    set Status = @Status
            //when not matched then  insert
            //    (UserId, TopicReplyId, Status) Values(@UserId, @TopicReplyId, @Status)
            //";

            var sql = $@" insert into TopicReplyLike (Id, UserId, TopicReplyId, Status) Values(@Id, @UserId, @TopicReplyId, @Status) ";
            return _dbContext.ExecuteUow(sql, like);
        }

        public int Update(TopicReplyLike like)
        {
            var sql = $@" update TopicReplyLike set Status = @Status, UpdateTime = getdate() where UserId = @UserId and TopicReplyId = @TopicReplyId ";
            return _dbContext.ExecuteUow(sql, like);
        }

        public IEnumerable<Guid> GetIdList(IEnumerable<Guid> topicReplyIds, Guid userId)
        {
            var sql = $@" select TopicReplyId from TopicReplyLike where UserId = @userId and TopicReplyId in @topicReplyIds and Status = 1 ";
            return _dbContext.Query<Guid>(sql, new { topicReplyIds, userId});
        }

        public IEnumerable<TopicReplyLike> GetPagination(Guid? topicReplyId, Guid? userId, int pageIndex, int pageSize)
        {
            Guid[] topicReplyIds = topicReplyId == null ? null : new Guid[] { topicReplyId.Value };
            Guid[] userIds = userId == null ? null : new Guid[] { userId.Value };
            return GetPagination(topicReplyIds, userIds, pageIndex, pageSize);
        }

        public IEnumerable<TopicReplyLike> GetPagination(IEnumerable<Guid> topicReplyIds, IEnumerable<Guid> userIds, int pageIndex, int pageSize)
        {
            var sql = $@" 
select 
    TRL.*,
    UI.NickName as UserName
from 
    TopicReplyLike TRL
    inner join {DbNameHelper.ISchoolUser}.[UserInfo] UI on UI.Id = TRL.UserId
where 
    TRL.Status = 1 
    and (@userIds is null or TRL.UserId in @userIds)
    and (@topicReplyIds is null or TRL.TopicReplyId in @topicReplyIds)
order by
    TRL.UpdateTime desc
offset (@pageIndex-1)*@pageSize rows
fetch next @pageSize rows only
";
            return _dbContext.Query<TopicReplyLike>(sql, new { topicReplyIds, userIds, pageIndex, pageSize });
        }

        /// <summary>
        /// 获取每个topic的前n条点赞
        /// </summary>
        /// <param name="topicReplyIds"></param>
        /// <param name="groupSize"></param>
        /// <returns></returns>
        public IEnumerable<TopicReplyLike> GetList(IEnumerable<Guid> topicReplyIds, int groupSize)
        {
            var sql = $@" 
SELECT 
	*
FROM
(
    select 
        TRL.*,
        UI.NickName as UserName,
        row_number() OVER(
	 	    partition BY  TRL.TopicReplyId
	 	    ORDER BY TRL.UpdateTime desc
	    ) AS rn
    from 
        TopicReplyLike TRL
        inner join {DbNameHelper.ISchoolUser}.[UserInfo] UI on UI.Id = TRL.UserId
    where 
        TRL.Status = 1 
        and TRL.TopicReplyId in @topicReplyIds
) AS T
where
   T.rn <= @groupSize
";
            return _dbContext.Query<TopicReplyLike>(sql, new { topicReplyIds, groupSize });
        }
    }
}
