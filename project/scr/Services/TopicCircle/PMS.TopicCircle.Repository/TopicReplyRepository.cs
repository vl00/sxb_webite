using PMS.TopicCircle.Domain.Entities;
using PMS.TopicCircle.Domain.Repositories;
using ProductManagement.Framework.MSSQLAccessor.DBContext;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static PMS.TopicCircle.Domain.Enum.GlobalEnum;

namespace PMS.TopicCircle.Repository
{
    public class TopicReplyRepository : Repository<TopicReply, TopicCircleDBContext>, ITopicReplyRepository
    {
        TopicCircleDBContext _dbContext;
        public TopicReplyRepository(TopicCircleDBContext dbContext) : base(dbContext)
        {
            this._dbContext = dbContext;
        }

        public int Add(TopicReply topicReply)
        {
            var sql = $@"
Insert into TopicReply
(
    Id, Depth, Content, TopicId, ParentId, 
    ParentUserId, Creator, Updator, FirstParentId
)
values
(
    @Id, @Depth, @Content, @TopicId, @ParentId, 
    @ParentUserId, @Creator,  @Updator, @FirstParentId
)
";
            return _dbContext.ExecuteUow(sql, topicReply);
        }

        public int Delete(TopicReply topicReply)
        {
            var sql = $@"
update TopicReply
set
    IsDeleted = 1
    , Updator = @Updator
    , UpdateTime = getdate()
where
    Id = @id
";
            return _dbContext.ExecuteUow(sql, topicReply);
        }

        public int UpdateContent(TopicReply topicReply)
        {
            var sql = $@"
update TopicReply
set 
    Content = @Content
    , Updator = @Updator
    , UpdateTime = getdate()
where
    Id = @id
";
            return _dbContext.ExecuteUow(sql, topicReply);
        }


        public int UpdateLikeTotal(Guid id, long total)
        {
            var sql = $@" update TopicReply set LikeCount = @total where Id = @id ";
            return _dbContext.ExecuteUow(sql, new { id, total });
        }

        public (IEnumerable<TopicReply> data, int total) GetPagination(Guid? topicId, Guid? parentId, Guid? firstParentId, int? depth, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize, bool createTimeDesc = false)
        {
            long offset = (pageIndex - 1) * pageSize;
            return GetPaginationByOffset(topicId, parentId, firstParentId, depth, startTime, endTime, offset, pageSize,  createTimeDesc );
        }

        public (IEnumerable<TopicReply> data, int total) GetPaginationByOffset(Guid? topicId, Guid? parentId, Guid? firstParentId, int? depth, DateTime? startTime, DateTime? endTime, long offset, int size, bool createTimeDesc = false)
        {
            var orderby = $"asc";
            if (createTimeDesc) orderby = "desc";
            var sql = $@" 
select 
    TR.*,
    UI.nickname as CreatorName,
    UI.headImgUrl as HeadImgUrl,
    PUI.nickname as ParentUserName
from 
    TopicReply TR
    left join {DbNameHelper.ISchoolUser}.userInfo UI on UI.id = TR.Creator
    left join {DbNameHelper.ISchoolUser}.userInfo PUI on PUI.id = TR.ParentUserId
where
    TR.IsDeleted = 0
    and (@depth is null or TR.Depth = @depth)
    and (@topicId is null or TR.TopicId = @topicId)
    and (@parentId is null or TR.ParentId = @parentId)
    and (@firstParentId is null or TR.FirstParentId = @firstParentId)
    and (@startTime is null or TR.CreateTime = @startTime)
    and (@endTime is null or TR.CreateTime = @endTime)
ORDER BY
	TR.CreateTime {orderby}, TR.TopicId
offset @offset rows
FETCH next @size rows only;

select 
	count(1) c
from 
    TopicReply TR
where
    TR.IsDeleted = 0
    and (@depth is null or TR.Depth = @depth)
    and (@topicId is null or TR.TopicId = @topicId)
    and (@parentId is null or TR.ParentId = @parentId)
    and (@firstParentId is null or TR.FirstParentId = @firstParentId)
    and (@startTime is null or TR.CreateTime = @startTime)
    and (@endTime is null or TR.CreateTime = @endTime);
";
            using (var grid = _dbContext.QueryMultiple(sql, new { topicId, parentId, firstParentId, depth, startTime, endTime, offset, size }))
            {
                var datas = grid.Read<TopicReply>();
                int total = grid.ReadFirstOrDefault<int>();
                return (datas, total);
            }

        }


        public IEnumerable<TopicReply> GetList(Guid topicId)
        {
            var sql = $@" 
select 
    TR.*,
    UI.nickname as CreatorName,
    UI.headImgUrl as HeadImgUrl,
    PUI.nickname as ParentUserName
from 
    TopicReply TR
    left join {DbNameHelper.ISchoolUser}.userInfo UI on UI.id = TR.Creator
    left join {DbNameHelper.ISchoolUser}.userInfo PUI on PUI.id = TR.ParentUserId
where 
    TR.IsDeleted = 0
    and TR.Depth != 0
    and TR.TopicId = @topicId
ORDER BY
	TR.CreateTime asc
";
            return _dbContext.Query<TopicReply>(sql, new { topicId });
        }

        /// <summary>
        /// 获取每个topic的前n条回复
        /// </summary>
        /// <param name="topicReplyIds"></param>
        /// <param name="userIds"></param>
        /// <param name="groupSize"></param>
        /// <returns></returns>
        public IEnumerable<TopicReply> GetTopList(IEnumerable<Guid> topicIds, int groupSize)
        {
            var sql = $@" 
SELECT 
	*
FROM
(
    select 
        TR.*,
        UI.nickname as CreatorName,
        UI.headImgUrl as HeadImgUrl,
        PUI.nickname as ParentUserName,
        row_number() OVER(
	 	    partition BY TR.TopicId
	 	    ORDER BY TR.CreateTime asc
	    ) AS rn
    from 
        TopicReply TR
        left join {DbNameHelper.ISchoolUser}.userInfo UI on UI.id = TR.Creator
        left join {DbNameHelper.ISchoolUser}.userInfo PUI on PUI.id = TR.ParentUserId
    where
        TR.IsDeleted = 0
        and TR.Depth != 0
        and TR.TopicId in @topicIds
) AS T
where
   T.rn <= @groupSize
";
            return _dbContext.Query<TopicReply>(sql, new { topicIds, groupSize });
        }

        public IEnumerable<TopicReply> GetLevelReplies(Guid replyID, int? maxDepth = null)
        {
            string sql = string.Format(@"
WITH _REPLIES AS (
	SELECT  TopicReply.* FROM TopicReply WHERE Id=@replyID AND IsDeleted=0
	UNION  ALL
	SELECT  TopicReply.* FROM _REPLIES, TopicReply WHERE TopicReply.Id=_REPLIES.PARENTID 
)
SELECT {0} 
    _REPLIES.* ,
    UI.nickname as CreatorName,
    UI.headImgUrl as HeadImgUrl,
    PUI.nickname as ParentUserName
FROM _REPLIES 
left join {1}.userInfo UI on UI.id = _REPLIES.Creator
left join {1}.userInfo PUI on PUI.id = _REPLIES.ParentUserId

", maxDepth == null?"":"TOP "+maxDepth, DbNameHelper.ISchoolUser);

            return _dbContext.Query<TopicReply>(sql, new { replyID });
        }

        public async Task<IEnumerable<TopicReply>> GetLevelRepliesEx(Guid replyID, int limit = 7)
        {
            var str_SQL = $@"WITH _REPLIES AS (
	                            SELECT
		                            1 AS [i] ,
		                            TopicReply.* 
	                            FROM
		                            TopicReply 
	                            WHERE
		                            Id = @replyID AND IsDeleted = 0 

	                            UNION ALL

		                        SELECT
			                        CASE WHEN TopicReply.IsDeleted = 1 
				                        THEN
					                        _REPLIES.i 
				                        ELSE 
                                            _REPLIES.i + 1 
			                        END ,
			                        TopicReply.* 
		                        FROM
			                        _REPLIES,
			                        TopicReply 
		                        WHERE
			                        TopicReply.Id = _REPLIES.PARENTID 
			                        AND _REPLIES.i < {limit}
                            ) 
			
                            SELECT
	                            _REPLIES.* ,
	                            UI.nickname AS CreatorName,
	                            UI.headImgUrl AS HeadImgUrl,
	                            PUI.nickname AS ParentUserName 
                            FROM
	                            _REPLIES
	                            LEFT JOIN iSchoolUser.dbo.userInfo UI ON UI.id = _REPLIES.Creator
	                            LEFT JOIN iSchoolUser.dbo.userInfo PUI ON PUI.id = _REPLIES.ParentUserId 
                            WHERE
	                            _REPLIES.IsDeleted = 0
                            ORDER BY
                                _REPLIES.Depth
                            --OFFSET 0 ROWS FETCH NEXT 10 ROWS ONLY";
            return await _dbContext.QueryAsync<TopicReply>(str_SQL, new { replyID });
        }
    }
}
