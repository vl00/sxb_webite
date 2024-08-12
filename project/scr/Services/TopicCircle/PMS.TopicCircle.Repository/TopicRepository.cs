using PMS.TopicCircle.Domain.Dtos;
using PMS.TopicCircle.Domain.Entities;
using PMS.TopicCircle.Domain.Repositories;
using ProductManagement.Framework.Foundation;
using ProductManagement.Framework.MSSQLAccessor.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static PMS.TopicCircle.Domain.Enum.GlobalEnum;

namespace PMS.TopicCircle.Repository
{
    public class TopicRepository : Repository<Topic, TopicCircleDBContext>, ITopicRepository
    {
        TopicCircleDBContext _dbContext;
        public TopicRepository(TopicCircleDBContext dbContext) : base(dbContext)
        {
            this._dbContext = dbContext;
        }

        public int Add(Topic topic)
        {
            var sql = $@"
Insert into Topic
(
    Id, CircleId, Content, OpenUserId, Type, 
    IsQA, Creator, Updator, LastReplyTime, IsAutoSync,
    CreateTime, UpdateTime, LastEditTime, DynamicTime
)
values
(
    @Id, @CircleId, @Content, @OpenUserId, @Type, 
    @IsQA, @Creator, @Updator, @LastReplyTime, @IsAutoSync,
    @CreateTime, @UpdateTime, @LastEditTime, @LastEditTime
)
";
            return _dbContext.ExecuteUow(sql, topic);
        }

        public int Delete(Topic topic)
        {
            var sql = $@"
update Topic
set 
    IsDeleted = 1
    , Updator = @Updator
    , UpdateTime = getdate()
where
    Id = @id
";
            return _dbContext.ExecuteUow(sql, topic);
        }

        public int Update(Topic topic)
        {
            var sql = $@"
update Topic
set 
    IsDeleted = 0
    , Content = @Content
    , OpenUserId = @OpenUserId
    , IsQA = @IsQA
    , Type = @Type
    , Updator = @Updator
    , UpdateTime = @UpdateTime
    -- , LastReplyTime = getdate()
    , LastEditTime = @LastEditTime
    , dynamicTime = @DynamicTime
where
    Id = @id
";
            return _dbContext.ExecuteUow(sql, topic);
        }

        public int UpdateContent(Topic topic)
        {
            var sql = $@"
update Topic
set 
    Content = @Content
    , Updator = @Updator
    , UpdateTime = getdate()
where
    Id = @id
";
            return _dbContext.ExecuteUow(sql, topic);
        }

        public int UpdateReplyTotal(Guid id, long total, DateTime? replyTime)
        {
            var sql = $@" update Topic set ReplyCount = @total, LastReplyTime = getdate(), UpdateTime = getdate(),dynamicTime = (CASE WHEN LastEditTime > @replyTime THEN LastEditTime ELSE @replyTime END) where Id = @id ";
            if (replyTime == null)
            {
                sql = $@" update Topic set ReplyCount = @total, UpdateTime = getdate() where Id = @id ";
                return _dbContext.ExecuteUow(sql, new { id, total });
            }
            else
            {
                return _dbContext.ExecuteUow(sql, new { id, total, replyTime });
            }
        }

        public int UpdateLikeTotal(Guid id, long total)
        {
            var sql = $@" update Topic set LikeCount = @total, UpdateTime = getdate() where Id = @id ";
            return _dbContext.ExecuteUow(sql, new { id, total });
        }

        public int UpdateFollowTotal(Guid id, long total)
        {
            var sql = $@" update Topic set FollowCount = @total, UpdateTime = getdate() where Id = @id ";
            return _dbContext.ExecuteUow(sql, new { id, total });
        }

        public SimpleTopicDto GetSimple(Guid id)
        {
            var sql = $@"
select 
    T.*,
    UI.NickName as UserName,
    UI.HeadImgUrl,
    C.Name as CircleName,
    C.Intro as CircleIntro,
    C.UserId as CircleUserId,
    case when C.UserId = T.Creator THEN 1 ELSE 0 END AS IsCircleOwner,
    (select Url from CircleCover CC where CC.CircleId = C.Id ) as CircleCover
from
    TopicView T
    left join {DbNameHelper.ISchoolUser}.UserInfo UI on T.Creator = UI.Id
    left join Circle C on C.Id = T.CircleId
where
    T.IsDeleted = 0
    and T.Status = 0 
    and T.Id = @id
";
            return _dbContext.Query<SimpleTopicDto>(sql, new { id }).FirstOrDefault();
        }

        public Topic Get(Guid id)
        {
            //Topic topic = _dbContext.Get<Topic, Guid>(id
            var sql = $@" select * from TopicView where IsDeleted = 0 and Status = 0 and Id = @id ";
            Topic topic = _dbContext.Query<Topic>(sql, new { id }).FirstOrDefault();

            if (topic != null)
            {
                //第一条评论id 即话题id
                var topicReplyId = id;
                var replySql = $@" 
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
    and TR.TopicId = @topicId
order by
	TR.CreateTime
    
";
                topic.Replies = _dbContext.Query<TopicReply>(replySql, new { topicId = id });


                var tagSql = $@" select * from TopicTag where TopicId = @topicId ";
                topic.Tags = _dbContext.Query<TopicTag>(tagSql, new { topicId = id });

                var imageSql = $@" select * from TopicReplyImage where IsDeleted = 0 and TopicReplyId = @topicReplyId Order by Sort ";
                topic.Images = _dbContext.Query<TopicReplyImage>(imageSql, new { topicReplyId });

                var attachmentSql = $@" select * from TopicReplyAttachment where IsDeleted = 0 and TopicReplyId = @topicReplyId";
                topic.Attachment = _dbContext.Query<TopicReplyAttachment>(attachmentSql, new { topicReplyId }).FirstOrDefault();

                var topicLikeCountSql = $"Select Count(1) from TopicReplyLike WHERE TopicReplyId = @topicID and Status = 1";
                topic.LikeCount = _dbContext.QuerySingle<int>(topicLikeCountSql, new { topicID = topic.Id });
            }
            return topic;
        }

        public Topic GetAutoSyncTopic(Guid attachId)
        {
            var sql = $@" 
select * from TopicView T
where 
    IsDeleted = 0
    and IsAutoSync = 1 
    and exists(
        select 1 from TopicReplyAttachment TRA where TRA.IsDeleted = 0 and TRA.TopicId = T.Id and TRA.AttachId = @attachId
    )
";
            return _dbContext.Query<Topic>(sql, new { attachId }).FirstOrDefault();
        }

        public long GetTotal(string keyword, Guid? circleId, Guid? creator, int? type, TopicTopType? topType, bool? isGood, bool? isQA, IEnumerable<int> tags, DateTime? startTime, DateTime? endTime)
        {
            keyword = string.Format("%{0}%", keyword);
            var sql = $@" 
select count(1) from Topic T
where
    IsDeleted = 0
    and Status = 0 
    and (@circleId is null or CircleId = @circleId)
    and (@creator is null or Creator = @creator)
    and (@type is null or @type & T.Type > 0)
    and (@topType is null or @topType & TopType > 0)
    and (@isGood is null or IsGood = @isGood)
    and (@isQA is null or IsQA = @isQA)
    and (@keyword is null or Content like @keyword)
    and (@tags is null or exists (
        select 1 from TopicTag TT where TT.TopicId = T.Id and TT.TagId in @tags
    ))
    and (@startTime is null or T.LastReplyTime >= @startTime)
    and (@endTime is null or T.LastReplyTime <= @endTime)
";
            return _dbContext.Query<long>(sql, new { keyword, circleId, creator, type, topType, isGood, isQA, tags, startTime, endTime }).FirstOrDefault();
        }

        public IEnumerable<SimpleTopicDto> GetList(IEnumerable<Guid> ids)
        {
            var sql = $@"
select 
    T.*,
    UI.NickName as UserName,
    UI.HeadImgUrl,
    C.Name as CircleName,
    C.Intro as CircleIntro,
    C.UserId as CircleUserId,
    case when C.UserId = T.Creator THEN 1 ELSE 0 END AS IsCircleOwner,
    (select Url from CircleCover CC where CC.CircleId = C.Id ) as CircleCover
from
    TopicView T
    left join {DbNameHelper.ISchoolUser}.UserInfo UI on T.Creator = UI.Id
    left join Circle C on C.Id = T.CircleId
where
    T.IsDeleted = 0
    and T.Status = 0 
    and T.Id in @ids
";
            return _dbContext.Query<SimpleTopicDto>(sql, new { ids });
        }

        public IEnumerable<SimpleTopicDto> GetList(Guid? circleId, Guid? creator, TopicTopType? topType, bool? isGood, bool? isQA)
        {
            var sql = $@"
select 
    T.*,
    UI.NickName as UserName,
    UI.HeadImgUrl,
    C.Name as CircleName,
    C.Intro as CircleIntro,
    C.UserId as CircleUserId,
    case when C.UserId = T.Creator THEN 1 ELSE 0 END AS IsCircleOwner,
    (select Url from CircleCover CC where CC.CircleId = C.Id ) as CircleCover
from
    TopicView T
    left join {DbNameHelper.ISchoolUser}.UserInfo UI on T.Creator = UI.Id
    left join Circle C on C.Id = T.CircleId
where
    T.IsDeleted = 0 
    and T.Status = 0 
    and (@circleIds is null or T.CircleId in @circleIds)
    and (@creator is null or T.Creator = @creator)
    and (@topType is null or @topType & T.TopType > 0)
    and (@isGood is null or T.IsGood = @isGood)
    and (@isQA is null or T.IsQA = @isQA)
ORDER BY
	T.Time DESC, T.CreateTime DESC
";
            return _dbContext.Query<SimpleTopicDto>(sql, new { circleId, creator, topType, isGood, isQA });
        }

        /// <summary>
        /// 公开话题 
        /// </summary>
        /// <param name="circleId"></param>
        /// <param name="topType"></param>
        /// <param name="sort"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IEnumerable<SimpleTopicDto> GetPagination(Guid? circleId, TopicTopType? topType, TopicSort sort = TopicSort.None, int pageIndex = 1, int pageSize = 10)
        {
            bool? isOpen = true;
            List<Guid> circleIds = null;
            if (circleId != null)
                circleIds = new List<Guid> { circleId.Value };
            return GetPagination(string.Empty, circleIds, null, null, topType, null, null, isOpen, null, null, null, sort, pageIndex, pageSize);
        }

        public IEnumerable<SimpleTopicDto> GetPagination(string keyword, Guid? circleId, Guid? creator, int? type, TopicTopType? topType, bool? isGood, bool? isQA, bool? isOpen,
            IEnumerable<int> tags, DateTime? startTime, DateTime? endTime, TopicSort sort = TopicSort.None, int pageIndex = 1, int pageSize = 10)
        {
            List<Guid> circleIds = null;
            if (circleId != null)
                circleIds = new List<Guid> { circleId.Value };
            return GetPagination(keyword, circleIds, creator, type, topType, isGood, isQA, isOpen, tags, startTime, endTime, sort, pageIndex, pageSize);
        }

        public IEnumerable<SimpleTopicDto> GetPagination(string keyword, IEnumerable<Guid> circleIds, Guid? creator, int? type, TopicTopType? topType, bool? isGood, bool? isQA, bool? isOpen,
            IEnumerable<int> tags, DateTime? startTime, DateTime? endTime, TopicSort sort = TopicSort.None, int pageIndex = 1, int pageSize = 10)
        {
            keyword = string.IsNullOrWhiteSpace(keyword) ? null : string.Format("%{0}%", keyword);
            var sortSql = sort.GetDescription() ?? "T.LastReplyTime  DESC";

            var openSql = "";
            if (isOpen != null)
            {
                openSql = true == isOpen ? " and T.OpenUserId is null " : " and T.OpenUserId is not null  ";
            }
            var circleSql = circleIds != null && circleIds.Any() ? " and T.CircleId in @circleIds " : "";

            var sql = $@" 
select
    T.*,
    UI.NickName as UserName,
    UI.HeadImgUrl,
    C.Name as CircleName,
    C.Intro as CircleIntro,
    C.UserId as CircleUserId,
    case when C.UserId = T.Creator THEN 1 ELSE 0 END AS IsCircleOwner,
    (select Url from CircleCover CC where CC.CircleId = C.Id ) as CircleCover
from
    TopicView T
    left join {DbNameHelper.ISchoolUser}.UserInfo UI on T.Creator = UI.Id
    left join Circle C on C.Id = T.CircleId
where
    T.IsDeleted = 0 
    and T.Status = 0 
    {circleSql}
    and (@creator is null or T.Creator = @creator)
    and (@type is null or @type & T.Type > 0)
    and (@topType is null or @topType & T.TopType > 0)
    and (@isGood is null or T.IsGood = @isGood)
    and (@isQA is null or T.IsQA = @isQA)
    and (@keyword is null or T.Content like @keyword)
    and (@tags is null or exists (
        select 1 from TopicTag TT where TT.TopicId = T.Id and TT.TagId in @tags
    ))
    and (@startTime is null or T.LastReplyTime >= @startTime)
    and (@endTime is null or T.LastReplyTime <= @endTime)
    {openSql}
ORDER BY
	{sortSql}
offset (@pageIndex - 1) * @pageSize rows
FETCH next @pageSize rows only
";
            return _dbContext.Query<SimpleTopicDto>(sql, new { keyword, circleIds, creator, type, topType, isGood, isQA, tags, startTime, endTime, pageIndex, pageSize });
        }

        public IEnumerable<SimpleTopicDto> GetPagination(Guid? circleId, TopicSort sort = TopicSort.None, int pageIndex = 1, int pageSize = 10)
        {
            bool? isOpen = true;
            List<Guid> circleIds = null;
            if (circleId != null)
                circleIds = new List<Guid> { circleId.Value };

            var sortSql = sort.GetDescription() ?? "T.LastReplyTime  DESC";

            var openSql = "";
            if (isOpen != null)
            {
                openSql = true == isOpen ? " and T.OpenUserId is null " : " and T.OpenUserId is not null  ";
            }
            var circleSql = circleIds != null && circleIds.Any() ? " and T.CircleId in @circleIds " : "";

            var sql = $@" 
select
    T.*,
    UI.NickName as UserName,
    UI.HeadImgUrl,
    C.Name as CircleName,
    C.Intro as CircleIntro,
    C.UserId as CircleUserId,
    case when C.UserId = T.Creator THEN 1 ELSE 0 END AS IsCircleOwner,
    (select Url from CircleCover CC where CC.CircleId = C.Id ) as CircleCover
from
    TopicView T
    left join {DbNameHelper.ISchoolUser}.UserInfo UI on T.Creator = UI.Id
    left join Circle C on C.Id = T.CircleId
where
    T.IsDeleted = 0 
    and C.IsDisable = 0
    and T.Status = 0 
    {circleSql}
    and (@topType is null or @topType & T.TopType > 0)
    {openSql}
ORDER BY
	{sortSql}
offset (@pageIndex - 1) * @pageSize rows
FETCH next @pageSize rows only
";
            return _dbContext.Query<SimpleTopicDto>(sql, new { circleIds, topType = TopicTopType.Admin, pageIndex, pageSize });
        }


        /// <summary>
        /// 公开话题
        /// <para>modified by labbor 同城人的话题 -> 同城圈子的话题</para>
        /// </summary>
        /// </summary>
        /// <param name="cityCode"></param>
        /// <param name="excludeIds"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IEnumerable<SimpleTopicDto> GetPagination(int? cityCode, IEnumerable<Guid> excludeIds, int pageIndex, int pageSize
            , bool banDisable = false)
        {

            string includeDisableSql = string.Empty;
            if (banDisable)
            {
                includeDisableSql = "and C.IsDisable = 0";
            }

            //排除的Id = excludeIds
            var sql = $@"
select 
    T.*,
    UI.NickName as UserName,
    UI.HeadImgUrl,
    C.Name as CircleName,
    C.Intro as CircleIntro,
    C.UserId as CircleUserId,
    case when C.UserId = T.Creator THEN 1 ELSE 0 END AS IsCircleOwner,
    (select Url from CircleCover CC where CC.CircleId = C.Id ) as CircleCover
from
    TopicView T
    left join Circle C on C.Id = T.CircleId
    left join {DbNameHelper.ISchoolUser}.UserInfo UI on UI.Id = C.UserId
where 
    T.IsDeleted = 0
    {includeDisableSql}
    and T.Status = 0 
    and T.OpenUserId is null 
    and (@cityCode is null or UI.City = @cityCode)
    and T.Id not in @excludeIds
ORDER BY
	T.LastReplyTime DESC, T.CreateTime DESC
offset (@pageIndex - 1) * @pageSize rows
FETCH next @pageSize rows only
";
            //首次发帖, LastReplyTime = CreateTime
            //LastReplyTime DESC 按帖子发布时间与最后回复时间降序排列； 
            //CreateTime DESC    如有同一时间发帖与回复的帖子，新发帖子 > 有新回复的帖子
            return _dbContext.Query<SimpleTopicDto>(sql, new { cityCode, excludeIds, pageIndex, pageSize });
        }





        public int Good(Guid id, bool isGood)
        {
            var good = isGood ? 1 : 0;
            var sql = $@"
update Topic
set 
    IsGood = @good
    , GoodTime = getdate()
    , UpdateTime = getdate()
where
    Id = @id
";
            return _dbContext.ExecuteUow(sql, new { id, good });
        }

        public int Top(Guid id, TopicTopType topType)
        {
            //置顶 0 无 1 圈主置顶  2 管理员置顶
            var sql = $@"
update Topic
set
    [TopType] = @topType
    , TopTime = getdate()
    , UpdateTime = getdate()
where
    Id = @id
";
            return _dbContext.ExecuteUow(sql, new { id, topType });
        }

        public IEnumerable<Topic> GetByIDs(IEnumerable<Guid> ids)
        {
            var str_SQL = $"Select * from Topic WHERE Id in @ids";
            return _dbContext.Query<Topic>(str_SQL, new { ids });
        }

        public async Task<IEnumerable<SimpleTopicDto>> GetByTopType(int type, int count = 10)
        {
            var str_Where = string.Empty;
            var str_Top = string.Empty;
            if (type >= 0) str_Where = $"AND T.TopType = @type";
            if (count > 0) str_Top = $"Top {count}";
            var str_SQL = $@"SELECT {str_Top}
	                            T.*,
	                            UI.NickName AS UserName,
	                            UI.HeadImgUrl,
	                            C.Name AS CircleName,
	                            C.Intro AS CircleIntro,
	                            C.UserId AS CircleUserId,
                                CASE WHEN C.UserId = T.Creator THEN 1 ELSE 0 END AS IsCircleOwner,
	                            ( SELECT Url FROM CircleCover CC WHERE CC.CircleId = C.Id ) AS CircleCover 
                            FROM
	                            TopicView T
	                            LEFT JOIN { DbNameHelper.ISchoolUser}.UserInfo UI ON T.Creator = UI.Id
	                            LEFT JOIN Circle C ON C.Id = T.CircleId 
                            WHERE
                                T.OpenUserId is not null,
	                            T.IsDeleted = 0 
	                            AND T.Status = 0
                                {str_Where}
                            ORDER BY
                                T.ReplyCount Desc,
	                            T.Time DESC,
	                            T.CreateTime DESC";
            return await _dbContext.QueryAsync<SimpleTopicDto>(str_SQL, new { type });
        }

        public async Task<IEnumerable<SimpleTopicDto>> GetByIsHandPick(bool isHandPick, int count = 10)
        {
            var str_Where = $"AND T.IsHandPick = 0";
            var str_Top = string.Empty;
            if (isHandPick) str_Where = $"AND T.IsHandPick = 1";
            if (count > 0) str_Top = $"Top {count}";
            var str_SQL = $@"SELECT {str_Top}
	                            T.*,
	                            UI.NickName AS UserName,
	                            UI.HeadImgUrl,
	                            C.Name AS CircleName,
	                            C.Intro AS CircleIntro,
	                            C.UserId AS CircleUserId,
                                CASE WHEN C.UserId = T.Creator THEN 1 ELSE 0 END AS IsCircleOwner,
	                            ( SELECT Url FROM CircleCover CC WHERE CC.CircleId = C.Id ) AS CircleCover 
                            FROM
	                            TopicView T
	                            LEFT JOIN { DbNameHelper.ISchoolUser}.UserInfo UI ON T.Creator = UI.Id
	                            LEFT JOIN Circle C ON C.Id = T.CircleId 
                            WHERE
                                T.OpenUserId is null
	                            AND T.IsDeleted = 0 
	                            AND T.Status = 0
                                {str_Where}
                            ORDER BY
	                            T.Time DESC,
                                T.ReplyCount Desc,
	                            T.CreateTime DESC";
            return await _dbContext.QueryAsync<SimpleTopicDto>(str_SQL, new { isHandPick });
        }





        public async Task<IEnumerable<SimpleTopicDto>> GetIsGood(int count = 10)
        {
            var str_Top = string.Empty;
            if (count > 0) str_Top = $"Top {count}";
            var str_SQL = $@"SELECT {str_Top}
	                            T.*,
	                            UI.NickName AS UserName,
	                            UI.HeadImgUrl,
	                            C.Name AS CircleName,
	                            C.Intro AS CircleIntro,
	                            C.UserId AS CircleUserId,
                                CASE WHEN C.UserId = T.Creator THEN 1 ELSE 0 END AS IsCircleOwner,
	                            ( SELECT Url FROM CircleCover CC WHERE CC.CircleId = C.Id ) AS CircleCover 
                            FROM
	                            TopicView T
	                            LEFT JOIN { DbNameHelper.ISchoolUser}.UserInfo UI ON T.Creator = UI.Id
	                            LEFT JOIN Circle C ON C.Id = T.CircleId 
                            WHERE
                                T.OpenUserId is null
	                            AND T.IsDeleted = 0 
	                            AND T.Status = 0
                                AND T.IsGood = 1
                            ORDER BY
	                            T.Time DESC,
                                T.ReplyCount Desc,
	                            T.CreateTime DESC";
            return await _dbContext.QueryAsync<SimpleTopicDto>(str_SQL, new { });
        }

        public IEnumerable<Topic> MoreDiscuss(Guid circleID, int offset, int limit)
        {
            string sql = @"	SELECT TopicView.*
	FROM TopicView
	WHERE 
	1=1
	AND 
	TopicView.CircleId=@circleID
	AND
	TopicView.IsDeleted = 0
	AND
	OpenUserId IS NULL
	ORDER BY
	ReplyCount DESC,
	TopicView.[TIME] DESC,
	NEWID()
	OFFSET @offset ROW
	FETCH NEXT @limit ROWS ONLY";
            return _dbContext.Query<Topic>(sql, new { circleID, offset, limit });
        }
        public async Task<IEnumerable<SimpleTopicDto>> GetTopicOrderByDynamicTime(Guid circleID, int count = 10)
        {
            var str_SQL = $@"SELECT TOP {count}
	                            * 
                            FROM
	                            topic 
                            WHERE
	                            CircleId = @circleID
                                AND IsDeleted = 0
                                AND Status = 0
                                AND OpenUserId is null
                            ORDER BY
	                            DynamicTime DESC";
            return await _dbContext.QueryAsync<SimpleTopicDto>(str_SQL, new { circleID });
        }

        public async Task<IEnumerable<Guid>> GetRelatedTopicIDs(Guid topicID, IEnumerable<int> tagIDs, int offset = 0, int limit = 5, Guid? circleID = null)
        {
            var str_WhereInCircle = circleID.HasValue ? $" AND Topic.CircleId = '{circleID}'" : string.Empty;
            var str_SQL = $@"SELECT
	                            TopicTag.TopicId AS [KEY],
	                            SUM (TopicTag.TagId) AS [Value] 
                            FROM
	                            TopicTag 
	                            left JOIN Topic on Topic.Id = TopicTag.TopicId
                            WHERE
	                            TopicTag.TagId IN @tagIDs
	                            AND TopicTag.TopicId != @topicID
                                AND Topic.OpenUserId is null
	                            {str_WhereInCircle}
                            GROUP BY
	                            TopicTag.TopicId
                            ORDER BY
	                            [Value] DESC 
                            OFFSET @offset ROWS FETCH NEXT @limit ROWS ONLY";
            var finds = await _dbContext.QueryAsync<KeyValuePair<Guid, int>>(str_SQL, new { tagIDs, topicID, offset, limit });
            if (finds?.Any() == true)
            {
                return finds.Select(p => p.Key);
            }
            return new Guid[0];
        }
    }
}

