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
    public class CollectionRepository : ICollectionRepository
    {
        private readonly UserDbContext _dbcontext;
        public CollectionRepository(UserDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public int GetCollectionCount(Guid userID)
        {
            return _dbcontext.Query<int>("select count(*) from collection where userID=@userID", new { userID }).FirstOrDefault();
        }
        public bool AddCollection(Guid userID, Guid dataID, byte dataType)
        {
            return _dbcontext.ExecuteUow(@"merge into collection
            using (select 1 as o) t
            on collection.dataID=@dataID and collection.userID=@userID
            when not matched then insert 
            (dataID, dataType, userID) values (@dataID, @dataType, @userID)
            when matched then update
            set time=sysdatetime();", new { dataID, dataType, userID }) > 0;
        }
        public bool RemoveCollection(Guid userID, Guid dataID)
        {
            return _dbcontext.Execute(@"delete from collection where dataID=@dataID and userID=@userID", new { dataID, userID }) > 0;
        }
        public bool IsCollected(Guid userID, Guid dataID, int dataType)
        {
            return _dbcontext.QuerySingle<int>(@"select count(*) from collection where dataID=@dataID and userID=@userID and dataType = @dataType;", new { dataID, userID, dataType }) > 0;
        }
        public bool IsCollected(Guid userID, Guid dataID)
        {
            return _dbcontext.QuerySingle<int>(@"select count(*) from collection where dataID=@dataID and userID=@userID;", new { dataID, userID}) > 0;
        }
        public List<Guid> GetUserCollection(Guid userID, byte dataType, int page = 1, int pageSize = 10)
        {
            return _dbcontext.Query<Guid>(@"select dataID from collection where 
            userID=@userID and dataType=@dataType
            order by time desc
            offset (@page-1)*@pageSize rows fetch next @pageSize row only",
            new { userID, dataType, page, pageSize }).AsList();
        }

        public List<Guid> GetPageUserCollection(Guid userID, byte dataType, ref int total, int page = 1, int pageSize = 10)
        {
            total = _dbcontext.Query<int>(@"select count(1) from collection where 
            userID=@userID and dataType=@dataType
            ",
            new { userID, dataType }).First();
            return _dbcontext.Query<Guid>(@"select dataID from collection where 
            userID=@userID and dataType=@dataType
            order by time desc
            offset (@page-1)*@pageSize rows fetch next @pageSize row only",
            new { userID, dataType, page, pageSize }).AsList();
        }

        public long GetCollectionTotal(Guid? userId, Guid? dataId, CollectionDataType[] dataTypes)
        {
            var param = new { userId, dataId, dataTypes };
            var total = _dbcontext.Query<int>($@"
select 
    count(1) 
from 
    collection C
    LEFT JOIN userInfo UI ON UI.id = C.dataId
    LEFT JOIN userInfo UI2 ON UI2.id = C.userId
where 
    C.dataType in @dataTypes
	and (dataType in (4,5) and UI.id is not null and UI2.id is not null)
    and (@userId is null or userId=@userId)
    and (@dataId is null or dataId=@dataId)
            ", param).First();
            return total;
        }

        public IEnumerable<Collection> GetCollections(Guid? userId, Guid? dataId, CollectionDataType[] dataTypes, int page = 1, int pageSize = 10)
        {
            var param = new { userId, dataId, dataTypes, page, pageSize };
            var data = _dbcontext.Query<Collection>(@"
select 
    *
from 
    collection C
    LEFT JOIN userInfo UI ON UI.id = C.dataId
    LEFT JOIN userInfo UI2 ON UI2.id = C.userId
where 
    dataType in @dataTypes
	and (dataType in (4,5) and UI.id is not null and UI2.id is not null)
    and (@userId is null or userId=@userId)
    and (@dataId is null or dataId=@dataId)
    order by time desc
    offset (@page-1)*@pageSize rows fetch next @pageSize row only
            ", param);
            return data;
        }

        public IEnumerable<CollectionDto> GetPagination(Guid[] userIds, Guid? dataId, CollectionDataType dataType, int pageIndex = 1, int pageSize = 10)
        {
            return GetPagination(userIds, dataId != null ? new Guid[] { dataId.Value } : null, new CollectionDataType[] { dataType }, pageIndex, pageSize);
        }

        public IEnumerable<CollectionDto> GetPagination(Guid[] userIds, Guid[] dataIds, CollectionDataType[] dataTypes, int pageIndex = 1, int pageSize = 10)
        {
            var param = new { userIds, dataIds, dataTypes, pageIndex, pageSize };
            var data = _dbcontext.Query<CollectionDto>(@"
select 
    C.*, UI.NickName as UserName
from 
    collection C
    LEFT JOIN userInfo UI ON UI.id = C.userId
where 
    dataType in @dataTypes
    and (@userIds is null or userId in @userIds)
    and (@dataIds is null or dataId in @dataIds)
order by time desc
offset (@pageIndex-1)*@pageSize rows 
fetch next @pageSize row only
            ", param);
            return data;
        }

        /// <summary>
        /// 这些userIds有没有关注dataId
        /// </summary>
        /// <param name="userIds"></param>
        /// <param name="dataId"></param>
        /// <returns></returns>
        public IEnumerable<Collection> GetCollections(Guid[] userIds, Guid dataId)
        {
            var param = new { userIds, dataId };
            var data = _dbcontext.Query<Collection>(@"
select 
    *
from 
    collection 
where 
    dataId=@dataId
    and userId in @userIds
            ", param);
            return data;
        }

        /// <summary>
        /// 这个userId有没有关注这些dataIds = Ids
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public List<Guid> GetCollection(List<Guid> ids, Guid UserId)
        {
            return _dbcontext.Query<Guid>(@"select dataID from collection where userID = @UserId and dataID in @ids",
                new { ids, UserId }).AsList();
        }

        public long GetCollectionTotal(List<Guid> ids)
        {
            return _dbcontext.QuerySingle<long>(@"select count(1) as total from collection where dataID in @ids",
                new { ids });
        }

        public FollowFansTotal GetUserFollowFans(Guid userId)
        {
            string sql = @"
select t.fans,t1.follow 
from 
(
    SELECT COUNT(1) AS fans, 1 as id
    FROM 
        collection C
        LEFT JOIN userInfo UI ON UI.id = C.userId
    WHERE dataType in(4,5) AND dataID = @userId and UI.id is not null
) as t
full JOIN 
(
    SELECT COUNT(1) AS follow, 1 as id
    FROM 
        collection C
        LEFT JOIN userInfo UI ON UI.id = C.dataID
    WHERE dataType in(4,5) AND userID = @userId and UI.id is not null
) as t1 on t.id = t1.id";
            return _dbcontext.Query<FollowFansTotal>(sql, new { userId }).FirstOrDefault();
        }

        /// <summary>
        /// <para>modified by Labbor on 20200722 修改达人动态文章和直播数量</para>
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public MydynamicTotal MydynamicTotal(Guid userId)
        {
            MydynamicTotal total = new MydynamicTotal();
            string sql = @"
SELECT SUM
	( t.total ) AS total
FROM
(
	SELECT COUNT ( 1 ) AS total
	FROM 
		[iSchoolArticle].[dbo].[Article_Talent] AT
		inner JOIN talent T ON T.id = AT.TalentId
	WHERE 
        T.user_id = @userId 

    UNION ALL
	SELECT COUNT ( 1 ) AS total
	FROM 
		[iSchoolLive].[dbo].[lector] L
		INNER JOIN [iSchoolLive].[dbo].[lecture_v2] LE ON LE.lector_id = L.Id
	WHERE 
       L.userId = @userId AND L.show = 1
	   AND LE.status IN (5) AND LE.show = 1
		 
    UNION ALL
	SELECT COUNT ( 1 ) AS total
	FROM [iSchoolProduct].[dbo].[SchoolComments] 
	WHERE 
        CommentUserId = @userId 

    UNION ALL
	SELECT COUNT( 1 ) AS total
	FROM [iSchoolProduct].[dbo].[QuestionInfos] 
	WHERE
		UserId = @userId 

    UNION ALL
	SELECT COUNT( 1 ) AS total
	FROM [iSchoolProduct].[dbo].[QuestionsAnswersInfos] 
	WHERE
		UserId = @userId 
		AND ParentId IS NULL 
) AS t


UNION ALL
SELECT COUNT ( 1 ) AS total 
FROM 
    collection C
    LEFT JOIN userInfo UI ON UI.id = C.dataId
WHERE dataType in @collectionDataType AND userID = @userId and UI.id is not null


UNION ALL
SELECT COUNT( 1 ) AS total
FROM
	history 
WHERE
	userID = @userId 
    and (isdelete = 0 or isdelete is null)
	AND dataType IN @messageDatType
	AND [time] >= DATEADD(
		MONTH,- 1,
	getdate()) 
AND [time] <= GETDATE() ";

            var collectionDataType = new CollectionDataType[] { CollectionDataType.UserLector, CollectionDataType.User };
            var messageDatType = new MessageDataType[] { MessageDataType.Article, MessageDataType.School, MessageDataType.Comment, MessageDataType.Question, MessageDataType.Lecture };
            var res = _dbcontext.Query<int>(sql, new { userId, collectionDataType, messageDatType }).ToList();
            total.Total = res[0];
            total.Follow = res[1];
            total.History = res[2];
            return total;
        }

    }
}
