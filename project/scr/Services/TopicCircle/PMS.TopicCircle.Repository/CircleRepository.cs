using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.TopicCircle.Repository
{
    using PMS.TopicCircle.Domain.Dtos;
    using PMS.TopicCircle.Domain.Entities;
    using PMS.TopicCircle.Domain.Repositories;
    using ProductManagement.Framework.MSSQLAccessor.DBContext;
    using System.Data.SqlTypes;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Threading.Tasks;

    public class CircleRepository : Repository<Circle, TopicCircleDBContext>, ICircleRepository
    {
        TopicCircleDBContext _dbContext;
        public CircleRepository(TopicCircleDBContext dBContext) : base(dBContext)
        {
            this._dbContext = dBContext;
        }

        public IEnumerable<Circle> GetCircles(Guid userId)
        {
            var circles = _dbContext.GetBy<Circle>(where: " [UserId] = @userId", param: new { userId }, order: null, fileds: null,null);
            return circles;
        }

        public Circle GetByUserId(Guid userId)
        {
            return GetBy(" UserId = @userId ", new { userId }).FirstOrDefault();
        }

        public IEnumerable<Circle> GetByFollower(CircleFollower follower)
        {
            return GetBy(@"EXISTS (SELECT 1 FROM CircleFollower where UserId = @userId AND Circle.Id = CircleFollower.CircleId) ",
                   new { userId = follower.UserId });
        }
        public async Task<(IEnumerable<Circle>, int)> GetByFollower(Guid userID, int offset = 0, int limit = 10)
        {
            var str_CountSQL = $@"SELECT
	                                Count(DISTINCT(c.Id))
                                FROM
	                                Circle AS c
	                                LEFT JOIN Topic AS t ON t.CircleId = c.Id 
	                                LEFT JOIN CircleFollower as f on f.CircleId = c.Id
	                            WHERE 
                                    f.UserId = @userID";

            var str_SQL = $@"SELECT
	                            MAX(t.dynamicTime) as DynamicTime,
	                            c.Id,
                                c.userID
                            FROM
	                            Circle AS c
	                            LEFT JOIN Topic AS t ON t.CircleId = c.Id 
	                            LEFT JOIN CircleFollower as f on f.CircleId = c.Id
	                            WHERE f.UserId = @userID
                            GROUP BY
	                            c.id,
                                c.userID
	                        ORDER BY DynamicTime DESC
                                OFFSET @offset ROWS FETCH NEXT @limit ROWS ONLY";
            return (await _dbContext.QueryAsync<Circle>(str_SQL, new { userID, offset, limit }), await _dbContext.QuerySingleAsync<int>(str_CountSQL, new { userID }));
        }

        public bool CreateTran(Circle entity, CircleCover cover)
        {
            return _dbContext.Tran<bool>((tran) =>
             {
                 var circleInsert = _dbContext.Insert(entity, tran);
                 var coverInsert = _dbContext.Insert(cover, tran);

                 if (circleInsert > 0 && coverInsert > 0)
                 {
                     return true;
                 }
                 else
                 {
                     return false;
                 }
             });
        }


        public bool EditTran(Circle entity, CircleCover cover)
        {
            return _dbContext.Tran<bool>((tran) =>
            {
                Circle circleUpdate = null;
                CircleCover coverUpdate = null;
                if (entity != null)
                {
                    circleUpdate = _dbContext.Update(entity, tran, new[] { "Intro", "Name", "BGColor", "ModifyTime" });
                }
                if (cover != null)
                {
                    coverUpdate = _dbContext.Update(cover, tran, new[] { "Url", "Modifytime" });
                }
                if (circleUpdate != null && coverUpdate != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            });

        }

        public IEnumerable<Circle> ExcuteUSP_STATISTICNEWSCIRCLE(int cityCode, int takeCount = 5)
        {
            IEnumerable<Circle> circles = _dbContext.Query<Circle>("USP_STATISTICNEWSCIRCLE", new { CITYCODE = cityCode, TAKECOUNT = takeCount }, null, true, null, System.Data.CommandType.StoredProcedure);
            return circles;
        }

        public IEnumerable<USPSTATICCIRCLENEWSINFO> ExcuteUSP_STATICCIRCLENEWSINFO(DateTime? timeNode, List<Guid> circleIds)
        {
            string[] circles = circleIds.Select(c => c.ToString()).ToArray();
            return _dbContext.Query<USPSTATICCIRCLENEWSINFO>("USP_STATICCIRCLENEWSINFO", new { TIMENODE = timeNode ?? SqlDateTime.MinValue.Value, CircleStrSplit = string.Join(",", circles) }, commandType: System.Data.CommandType.StoredProcedure);
        }

        IEnumerable<USPQUERYMYCIRCLESDto> ICircleRepository.ExcuteUSP_QUERYMYCIRCLES(DateTime timeNode, Guid userId)
        {
            return _dbContext.Query<USPQUERYMYCIRCLESDto>("USP_QUERYMYCIRCLES", new { TIMENODE = timeNode, USERID = userId }, commandType: System.Data.CommandType.StoredProcedure);
        }

        public IEnumerable<SimpleCircleDto> GetList(IEnumerable<Guid> ids)
        {
            var sql = $@" 
select 
    C.*,
    UI.NickName as UserName
from
    Circle C
    left join {DbNameHelper.ISchoolUser}.UserInfo UI on C.UserId = UI.Id
where
    C.Id in @ids
";
            return _dbContext.Query<SimpleCircleDto>(sql, new { ids });
        }

        public async Task<IEnumerable<Circle>> GetList(int count = 8, int provinceCode = 0, int cityCode = 0, IEnumerable<Guid> notInIDs = null)
        {
            var str_Order = string.Empty;
            var str_Top = string.Empty;
            if (count > 0)
            {
                str_Top = $" Top {count} ";
            }
            var str_Where = "1 = 1";
            if (provinceCode > 0)
            {
                str_Where = $"u.city LIKE '{provinceCode}%'";
            }
            else if (cityCode > 0)
            {
                //str_Order = $"case WHEN u.city = @cityCode then 0 else 1 end,";
                str_Where = $"u.city = @cityCode";
            }
            if (notInIDs?.Any() == true)
            {
                str_Where += $" And c.Id not in @notInIDs";
            }
            var str_SQL = $@"SELECT
                                {str_Top}
	                            c.*,
                                u.city,
                                u.NickName as UserName
                            FROM
	                            [dbo].[Circle] AS C
	                            LEFT JOIN iSchoolUser.dbo.Userinfo AS U ON c.UserId = u.Id 
                            WHERE
	                            {str_Where}
                            ORDER BY
                                {str_Order}
	                            c.CreateTime DESC";
            return await _dbContext.QueryAsync<Circle>(str_SQL, new { cityCode, notInIDs });
        }
        public async Task<IEnumerable<Circle>> GetList(int count = 8, int provinceCode = 0)
        {

            var str_Top = string.Empty;
            if (count > 0)
            {
                str_Top = $" Top {count} ";
            }
            var str_Where = "1 = 1";
            if (provinceCode > 0)
            {
                str_Where = $"u.city LIKE '{provinceCode}%'";
            }
            var str_SQL = $@"SELECT
                                {str_Top}
	                            c.* 
                            FROM
	                            [dbo].[Circle] AS C
	                            LEFT JOIN iSchoolUser.dbo.Userinfo AS U ON c.UserId = u.Id 
                            WHERE
	                            {str_Where}
                            ORDER BY
	                            c.CreateTime DESC";
            return await _dbContext.QueryAsync<Circle>(str_SQL, new { });
        }
        public int TopicCount(Guid circleID)
        {
            string sql = @"SELECT COUNT(1) C FROM Topic
JOIN Circle ON Topic.CircleId = Circle.Id
WHERE
CircleId = @circleID
AND
(OpenUserId IS NULL OR OpenUserId != Circle.UserId)
AND
IsDeleted = 0
AND
[Status] = 0";
            return _dbContext.ExecuteScalar<int>(sql, new { circleID });



        }

        public async Task<IEnumerable<CircleStatisticsDto>> GetStatistics(IEnumerable<Guid> circleIDs)
        {
            var str_SQL = $@"SELECT COUNT
	                            ( t.Id ) AS topicCount,
	                            SUM ( t.ReplyCount ) AS ReplyCount,
	                            SUM ( t.LikeCount ) AS LikeCount,
	                            COUNT ( t.id ) + SUM ( t.ReplyCount ) + SUM ( t.LikeCount ) AS DynamicCount,
	                            c.Id 
                            FROM
	                            Circle AS c
	                            LEFT JOIN Topic AS t ON t.CircleId = c.Id 
                            WHERE
	                            t.IsDeleted = 0 AND c.ID in @circleIDs
                            GROUP BY
	                            c.id 
                            ORDER BY
                                DynamicCount DESC";
            //OFFSET {offset} ROWS FETCH NEXT {limit} ROWS ONLY";
            return await _dbContext.QueryAsync<CircleStatisticsDto>(str_SQL, new { circleIDs });
        }

        public async Task<IEnumerable<SimpleCircleDto>> GetSelfFirstCircles(Guid userID, int offset = 0, int limit = 10, string orderBy = null)
        {
            var str_Order = @"CASE c.UserId 
		                            WHEN @userID THEN 1 ELSE 0 
	                            END DESC, ";
            if (!string.IsNullOrEmpty(orderBy))
            {
                str_Order += orderBy;
            }
            else
            {
                str_Order += "IsFollowed DESC ";
            }
            var str_SQL = $@"SELECT
                                CASE
                                    WHEN cf.Id IS NOT NULL THEN 1 ELSE 0 
	                            END AS IsFollowed ,
	                            c.* 
                            FROM
	                            Circle AS c
	                            LEFT JOIN CircleFollower cf ON cf.UserId = @userID 
	                            AND cf.CircleId = c.Id 
                            ORDER BY
                                {str_Order}
                            OFFSET @offset ROWS FETCH NEXT @limit ROWS ONLY";
            return await _dbContext.QueryAsync<SimpleCircleDto>(str_SQL, new { userID, offset, limit });
        }

        public async Task<int> Count(string where)
        {
            var str_SQL = $@"Select Count(id) From Circle";
            if (!string.IsNullOrEmpty(where)) str_SQL += $" {where}";
            return await _dbContext.QuerySingleAsync<int>(str_SQL, new { });
        }

        public async Task<IEnumerable<SimpleCircleDto>> GetCircleOrderByYesterdayActive(Guid? userID, int offset = 0, int limit = 10)
        {
            var currentTime = DateTime.Now.Date;
            var str_SQL = $@"SELECT 
	                            COUNT	( t.id ) AS topicCount,
	                            SUM ( t.ReplyCount ) AS ReplyCount,
	                            COUNT ( t.id ) + SUM ( t.ReplyCount ) AS dynamicCount,
	                            c.Id ,
                                c.UserId,
	                            c.followCount 
                            FROM
	                            Circle AS c
	                            LEFT JOIN Topic AS t ON t.CircleId = c.Id 
	                            AND t.CreateTime > @startTime 
	                            AND t.CreateTime < @endTime
	                            LEFT JOIN TopicReply AS r ON r.TopicId = t.Id 
	                            AND r.CreateTime > @startTime
	                            AND r.CreateTime < @endTime
                            GROUP BY
	                            c.Id,
                                c.UserId,
	                            c.followCount 
                            ORDER BY
                                CASE c.UserId 
		                            WHEN @userID THEN 1 ELSE 0 
	                            END DESC,
	                            DynamicCount DESC,
	                            topicCount DESC,
	                            c.followCount DESC
                            OFFSET @offset ROWS FETCH NEXT @limit ROWS ONLY";
            var finds = await _dbContext.QueryAsync<CircleStatisticsDto>(str_SQL, new { startTime = currentTime.AddDays(-1), endTime = currentTime, offset, limit ,userID});
            if (finds?.Any() == true)
            {
                if (!userID.HasValue) userID = Guid.Empty;
                var ids = finds.Select(p => p.Id);
                str_SQL = $@"SELECT
                                CASE
                                    WHEN cf.Id IS NOT NULL THEN 1 ELSE 0 
	                            END AS IsFollowed ,
	                            c.* 
                            FROM
	                            Circle AS c
	                            LEFT JOIN CircleFollower cf ON cf.UserId = @userID AND cf.CircleId = c.Id
                            WHERE
                                c.id in @ids";
                var result = await _dbContext.QueryAsync<SimpleCircleDto>(str_SQL, new { userID, ids });
                if (result?.Any() == true)
                {
                    return result.OrderBy(p => Array.IndexOf(ids.ToArray(), p.Id));
                }

            }
            return new List<SimpleCircleDto>();
        }

        public async Task<int> UpdateCircleCountData()
        {
            var str_SQL = @"UPDATE Circle SET 
                                ReplyCount = ( SELECT COALESCE ( SUM ( ReplyCount ), 0 ) FROM Topic WHERE Topic.CircleId = Circle.Id ),
                                TopicCount = ( SELECT COUNT ( 1 ) FROM Topic WHERE Topic.CircleId = Circle.Id ),
                                FollowCount = ( SELECT COUNT ( 1 ) FROM CircleFollower WHERE CircleFollower.CircleId = Circle.Id );";
            return await _dbContext.ExecuteAsync(str_SQL, new { });
        }

        public int YesterdayTopicCount(Guid circleID)
        {
            string sql = @"DECLARE @DATENOW DATE = GETDATE();
SELECT COUNT(1) FROM Topic 
WHERE 
CircleId=@circleID
AND
Status=0
AND 
IsDeleted = 0
AND
CreateTime BETWEEN DATEADD(DAY, -1,@DATENOW) AND @DATENOW
";
            return _dbContext.ExecuteScalar<int>(sql, new { circleID });
        }

        public int YesterdayFollowerCount(Guid circleID)
        {
            string sql = @"
DECLARE @DATENOW DATE = GETDATE();
SELECT COUNT(1) FROM CircleFollower
WHERE 
CircleId=@circleID
AND
[Time] BETWEEN DATEADD(DAY, -1,@DATENOW) AND @DATENOW";
            return _dbContext.ExecuteScalar<int>(sql, new { circleID });
        }


        public async Task<IEnumerable<dynamic>> ExportStaticData(DateTime btime,DateTime etime)
        {
            

            string stastic = @"USE iSchoolTopicCircle
DELETE StaticData
INSERT StaticData(CircleID,CircleName) SELECT Id,NAME FROM Circle
SELECT * FROM StaticData
--================================================统计3.0==================================================================--
USE iSchoolUser
--拿出灌水用户
SELECT * INTO #GUANSHUIUSER FROM userInfo WHERE mobile LIKE '12%'
--SELECT * FROM #GUANSHUIUSER

USE iSchoolTopicCircle
SELECT * INTO #TARGETCIRCLE FROM Circle
--SELECT * FROM #TARGETCIRCLE
--INSERT INTO StaticData(CircleID,CircleName)  (select Circle.Id,Circle.Name from Circle)
--SELECT * FROM StaticData

--关注话题圈的新用户列表中
USE iSchoolTopicCircle
SELECT UserId,MIN(Time) T INTO #NEWUSER FROM CircleFollower WHERE Time>@btime AND TIME<@etime  GROUP BY UserId
--求得新关注话题圈的用户集合
SELECT * into #NewCircleFollowers FROM CircleFollower
WHERE EXISTS(SELECT 1 FROM #NEWUSER WHERE CircleFollower.UserId=#NEWUSER.UserId AND CircleFollower.[Time]=#NEWUSER.T)
--SELECT * FROM #NewCircleFollowers

--求得关注服务号的用户集合
USE iSchoolUser
 SELECT openid_weixin.openID,valid,appName,userID,SubscribeTime into #OPENIDS FROM openid_weixin 
 JOIN WXUSERINFO ON WXUSERINFO.OPENID=openid_weixin.openID
 --SELECT * FROM #OPENIDS

--求得新关注服务号且为话题圈新关注用户的用户集合
USE iSchoolUser
SELECT #NewCircleFollowers.CIRCLEID,#TARGETCIRCLE.NAME, COUNT(userInfo.id) c INTO #T1  FROM  userInfo
JOIN #NewCircleFollowers ON userInfo.id=#NewCircleFollowers.USERID
JOIN #TARGETCIRCLE ON #NewCircleFollowers.CIRCLEID = #TARGETCIRCLE.ID
WHERE
 userInfo.id IN  (SELECT userID FROM #OPENIDS  WHERE SubscribeTime between @btime and @etime AND  appName='FWH' AND valid=1)
 AND
 userInfo.id IN (SELECT USERID FROM #NewCircleFollowers)
 AND
 userInfo.id NOT IN  (SELECT ID FROM #GUANSHUIUSER) 
GROUP BY #NewCircleFollowers.CIRCLEID,#TARGETCIRCLE.NAME
--SELECT * FROM  #T1 

----------插入统计表
	    USE iSchoolTopicCircle
		UPDATE StaticData SET NewUserCount = (SELECT c FROM #T1 WHERE CircleID = StaticData.CircleID)
		WHERE EXISTS(SELECT 1 FROM #T1 WHERE CircleID = StaticData.CircleID)
-- 		SELECT * FROM STATICDATA
--求得旧关注服务号，新关注话题圈的用户集合
USE iSchoolUser
SELECT #NewCircleFollowers.CIRCLEID,#TARGETCIRCLE.NAME, COUNT(userInfo.id) c into #T2
 FROM  userInfo
JOIN #NewCircleFollowers ON userInfo.id=#NewCircleFollowers.USERID
JOIN #TARGETCIRCLE ON #NewCircleFollowers.CIRCLEID = #TARGETCIRCLE.ID
WHERE
 userInfo.id IN  (SELECT userID FROM #OPENIDS  WHERE SubscribeTime < @btime AND  appName='FWH' AND valid=1)
 AND
 userInfo.id IN (SELECT USERID FROM #NewCircleFollowers)
 AND
 userInfo.id NOT IN  (SELECT ID FROM #GUANSHUIUSER) 
GROUP BY #NewCircleFollowers.CIRCLEID,#TARGETCIRCLE.NAME
--SELECT * FROM #T2
----------插入统计表
	  USE iSchoolTopicCircle
		UPDATE StaticData SET SubscribeUserCount = (SELECT c FROM #T2 WHERE CircleID = StaticData.CircleID)
		WHERE EXISTS(SELECT 1 FROM #T2 WHERE CircleID = StaticData.CircleID)
-- 		SELECT * FROM STATICDATA



-- declare @btime datetime='2020-11-30 00:00:00' 
-- declare @etime datetime = '2020-12-6 23:59:59'
-- --求得留存用户数
-- SELECT Circle.Id,Circle.Name,COUNT(CircleFollower.UserId) C INTO #T3 FROM [10.1.0.15].iSchoolTopicCircle.dbo.CircleFollower
-- JOIN [10.1.0.15].iSchoolTopicCircle.dbo.Circle ON CircleFollower.CircleId = Circle.Id
-- WHERE 
-- CircleFollower.Time between @btime and @etime
-- AND
-- CircleFollower.UserId NOT IN ( SELECT Id FROM [10.1.0.15].[ISCHOOLUSER].[DBO].userInfo WHERE mobile LIKE '12%')
-- AND 
-- EXISTS (
-- SELECT 1 FROM [ISCHOOLLOGS].[DBO].[SxbLogs]  WHERE  [SxbLogs].UserId=CONVERT(varchar(50),CircleFollower.UserId) AND [SxbLogs].[PLATFORM]=2 AND [SxbLogs].[Time]  BETWEEN DATEADD(HOUR,12,CircleFollower.[Time] )AND DATEADD(HOUR,168,CircleFollower.[Time] ) 
-- )
-- GROUP BY Circle.Id,Circle.Name
-- SELECT * FROM #T3
-- ----------插入统计表
-- 		UPDATE  [10.1.0.15].iSchoolTopicCircle.[dbo].StaticData SET STAYUSERCOUNT = (SELECT c FROM #T3 WHERE ID = StaticData.CircleID)
-- 		WHERE EXISTS(SELECT 1 FROM #T3 WHERE ID = StaticData.CircleID)
-- 		SELECT * FROM [10.1.0.15].iSchoolTopicCircle.[dbo].StaticData


--求得圈内用户发帖总集合
USE iSchoolTopicCircle
SELECT  Circle.Id,Circle.Name,COUNT(Topic.Id) C INTO #T4 FROM Topic
JOIN Circle ON Topic.CircleId = Circle.Id
WHERE
Topic.CreateTime between @btime and @etime
AND 
Creator NOT  IN (SELECT UserId FROM Circle)
AND
IsDeleted=0
GROUP BY Circle.Id,Circle.Name
--SELECT * FROM #T4
----------插入统计表
		UPDATE  StaticData SET UserTopicCount = (SELECT c FROM #T4 WHERE ID = StaticData.CircleID)
		WHERE EXISTS(SELECT 1 FROM #T4 WHERE ID = StaticData.CircleID)
-- 		SELECT * FROM StaticData

--求得圈内真实用户发帖总集合
USE iSchoolTopicCircle
SELECT  Circle.Id,Circle.Name,COUNT(Topic.Id) C INTO #T5 FROM Topic
JOIN Circle ON Topic.CircleId = Circle.Id
WHERE
Topic.CreateTime between @btime and @etime
AND 
Creator NOT  IN (SELECT UserId FROM Circle)
AND 
Creator NOT IN (SELECT Id FROM #GUANSHUIUSER)
AND 
IsDeleted=0
GROUP BY Circle.Id,Circle.Name
--SELECT * FROM #T5
----------插入统计表
		UPDATE  StaticData SET UserTopicCountReal = (SELECT c FROM #T5 WHERE ID = StaticData.CircleID)
		WHERE EXISTS(SELECT 1 FROM #T5 WHERE ID = StaticData.CircleID)
-- 		SELECT * FROM StaticData


--求得圈内用户点赞集合
SELECT Circle.Id,Circle.Name, COUNT(TopicReplyLike.Id) C INTO #T6 FROM TopicReplyLike
JOIN TopicReply ON TopicReply.Id = TopicReplyLike.TopicReplyId
JOIN Topic ON Topic.Id=TopicReply.TopicId
JOIN Circle ON Circle.Id = Topic.CircleId
WHERE
Topic.IsDeleted = 0
AND 
TopicReply.IsDeleted= 0
AND
TopicReplyLike.[Status]=1
AND
TopicReplyLike.UserId NOT IN (SELECT UserId FROM Circle)
AND
TopicReplyLike.UpdateTime between @btime and @etime
GROUP BY Circle.Id,Circle.Name
--SELECT * FROM #T6
----------插入统计表
		UPDATE  StaticData SET UserLikeCount = (SELECT c FROM #T6 WHERE ID = StaticData.CircleID)
		WHERE EXISTS(SELECT 1 FROM #T6 WHERE ID = StaticData.CircleID)
-- 		SELECT * FROM StaticData


--求得圈内真实用户点赞集合
SELECT Circle.Id,Circle.Name, COUNT(TopicReplyLike.Id) C INTO #T7 FROM TopicReplyLike
JOIN TopicReply ON TopicReply.Id = TopicReplyLike.TopicReplyId
JOIN Topic ON Topic.Id=TopicReply.TopicId
JOIN Circle ON Circle.Id = Topic.CircleId
WHERE
Topic.IsDeleted = 0
AND 
TopicReply.IsDeleted= 0
AND
TopicReplyLike.[Status]=1
AND
TopicReplyLike.UserId NOT IN (SELECT UserId FROM Circle)
AND
TopicReplyLike.UpdateTime between @btime and @etime
AND 
TopicReplyLike.UserId NOT IN (SELECT Id FROM #GUANSHUIUSER)
GROUP BY Circle.Id,Circle.Name
--SELECT * FROM #T7
----------插入统计表
		UPDATE  StaticData SET UserLikeCountReal = (SELECT c FROM #T7 WHERE ID = StaticData.CircleID)
		WHERE EXISTS(SELECT 1 FROM #T7 WHERE ID = StaticData.CircleID)
-- 		SELECT * FROM StaticData

--求得圈内用户收藏话题集合
SELECT Circle.Id,Circle.Name ,COUNT(collection.dataID) C INTO #T8 FROM iSchoolUser.dbo.[collection] 
JOIN Topic ON [collection].dataID = Topic.Id
JOIN Circle ON Circle.Id=Topic.CircleId
WHERE 
Topic.IsDeleted=0
AND
[collection].time between @btime and @etime
AND 
[collection].dataType=7
AND 
[collection].userID NOT IN (SELECT UserId FROM Circle)
GROUP BY Circle.Id,Circle.Name
--SELECT * FROM #T8
----------插入统计表
		UPDATE  StaticData SET UserCollectCount = (SELECT c FROM #T8 WHERE ID = StaticData.CircleID)
		WHERE EXISTS(SELECT 1 FROM #T8 WHERE ID = StaticData.CircleID)
-- 		SELECT * FROM StaticData

--求得圈内真实用户收藏话题集合
SELECT Circle.Id,Circle.Name ,COUNT(collection.dataID) c into #T9 FROM iSchoolUser.dbo.[collection] 
JOIN Topic ON [collection].dataID = Topic.Id
JOIN Circle ON Circle.Id=Topic.CircleId
WHERE 
Topic.IsDeleted=0
AND
[collection].time between @btime and @etime
AND 
[collection].dataType=7
AND 
[collection].userID NOT IN (SELECT UserId FROM Circle)
AND
[collection].userID NOT IN (SELECT Id FROM #GUANSHUIUSER)
GROUP BY Circle.Id,Circle.Name
--SELECT * FROM #T9
----------插入统计表
		UPDATE  StaticData SET UserCollectCountReal = (SELECT c FROM #T9 WHERE ID = StaticData.CircleID)
		WHERE EXISTS(SELECT 1 FROM #T9 WHERE ID = StaticData.CircleID)
-- 		SELECT * FROM StaticData

--求得圈内用户提问集合
USE iSchoolTopicCircle
SELECT Circle.Id,Circle.Name,COUNT(Topic.Id) c into #T10  FROM Topic
JOIN Circle ON Topic.CircleId = Circle.Id
WHERE
IsQA=1 
AND
Topic.CreateTime between @btime and @etime
AND
IsDeleted=0 
AND
Creator NOT IN (SELECT UserId FROM Circle)
GROUP BY Circle.Id,Circle.Name
--SELECT * FROM #T10
----------插入统计表
		UPDATE  StaticData SET UserQACount = (SELECT c FROM #T10 WHERE ID = StaticData.CircleID)
		WHERE EXISTS(SELECT 1 FROM #T10 WHERE ID = StaticData.CircleID)
-- 		SELECT * FROM StaticData



--求得圈内真实用户提问集合
USE iSchoolTopicCircle
SELECT Circle.Id,Circle.Name,COUNT(Topic.Id) c into #T11  FROM Topic
JOIN Circle ON Topic.CircleId = Circle.Id
WHERE
IsQA=1 
AND
Topic.CreateTime between @btime and @etime
AND
IsDeleted=0 
AND
Creator NOT IN (SELECT UserId FROM Circle)
AND
Creator NOT IN (SELECT Id FROM #GUANSHUIUSER)
GROUP BY Circle.Id,Circle.Name
--SELECT * FROM #T11
----------插入统计表
		UPDATE  StaticData SET UserQACountReal = (SELECT c FROM #T11 WHERE ID = StaticData.CircleID)
		WHERE EXISTS(SELECT 1 FROM #T11 WHERE ID = StaticData.CircleID)
-- 		SELECT * FROM StaticData


--求得圈内用户评论数
SELECT  Circle.Id,Circle.Name ,COUNT(TopicReply.Id) c into #T12  FROM TopicReply 
JOIN Topic ON Topic.Id = TopicReply.TopicId
JOIN Circle ON Circle.Id = Topic.CircleId
WHERE 
TopicReply.Creator NOT IN (SELECT UserId FROM Circle)
AND
TopicReply.CreateTime  between @btime and @etime
AND
Depth>0 
AND
TopicReply.IsDeleted=0
GROUP BY Circle.Id,Circle.Name
--SELECT * FROM #T12
----------插入统计表
		UPDATE  StaticData SET UserCommentCount = (SELECT c FROM #T12 WHERE ID = StaticData.CircleID)
		WHERE EXISTS(SELECT 1 FROM #T12 WHERE ID = StaticData.CircleID)
-- 		SELECT * FROM StaticData



--求得圈内真实用户评论数
SELECT  Circle.Id,Circle.Name ,COUNT(TopicReply.Id) c into #T13  FROM TopicReply 
JOIN Topic ON Topic.Id = TopicReply.TopicId
JOIN Circle ON Circle.Id = Topic.CircleId
WHERE 
TopicReply.Creator NOT IN (SELECT UserId FROM Circle)
AND
TopicReply.Creator NOT IN (SELECT Id FROM #GUANSHUIUSER)
AND
TopicReply.CreateTime between @btime and @etime
AND
Depth>0 
AND
TopicReply.IsDeleted=0
GROUP BY Circle.Id,Circle.Name
--SELECT * FROM #T13
----------插入统计表
		UPDATE  StaticData SET UserCommentCountReal = (SELECT c FROM #T13 WHERE ID = StaticData.CircleID)
		WHERE EXISTS(SELECT 1 FROM #T13 WHERE ID = StaticData.CircleID)
-- 		SELECT * FROM StaticData



--圈内圈主发帖总数
USE iSchoolTopicCircle
SELECT Circle.Id,Circle.[Name],COUNT(TopicReply.Id) c into #T14  FROM TopicReply 
JOIN Topic ON Topic.Id = TopicReply.TopicId
JOIN Circle ON Topic.CircleId = Circle.Id
WHERE 
TopicReply.CreateTime  between @btime and @etime
AND
Depth=0
AND
TopicReply.IsDeleted=0 
AND
TopicReply.Creator IN (SELECT UserId FROM Circle)
GROUP BY Circle.Id,Circle.[Name]
--SELECT * FROM #T14
----------插入统计表
		UPDATE  StaticData SET MasterTopicCount = (SELECT c FROM #T14 WHERE ID = StaticData.CircleID)
		WHERE EXISTS(SELECT 1 FROM #T14 WHERE ID = StaticData.CircleID)
-- 		SELECT * FROM StaticData



--圈主有效回复数
SELECT Circle.Id,Circle.Name,COUNT(TopicReply.Id) c into #T15 FROM TopicReply
JOIN Topic ON TopicReply.TopicId = Topic.Id AND  Topic.IsQA = 1 AND Topic.IsDeleted = 0
JOIN Circle ON Circle.Id = Topic.CircleId
WHERE 
TopicReply.Creator IN (SELECT UserId FROM Circle)
AND
TopicReply.CreateTime between @btime and @etime
AND 
TopicReply.IsDeleted = 0
AND 
LEN(TopicReply.Content)>=50
AND
TopicReply.CreateTime BETWEEN Topic.CreateTime AND  DATEADD(HOUR,48,Topic.CreateTime)
GROUP BY Circle.Id,Circle.Name
--SELECT * FROM #T15
----------插入统计表
		UPDATE  StaticData SET MasterReplyCountValid = (SELECT c FROM #T15 WHERE ID = StaticData.CircleID)
		WHERE EXISTS(SELECT 1 FROM #T15 WHERE ID = StaticData.CircleID)
-- 		SELECT * FROM StaticData

--圈主无效回复数
SELECT Circle.Id,Circle.Name,COUNT(TopicReply.Id) c into #T16 FROM TopicReply
JOIN Topic ON TopicReply.TopicId = Topic.Id AND  Topic.IsQA = 1 AND Topic.IsDeleted = 0
JOIN Circle ON Circle.Id = Topic.CircleId
WHERE 
TopicReply.Creator IN (SELECT UserId FROM Circle)
AND
TopicReply.CreateTime between @btime and @etime
AND 
TopicReply.IsDeleted = 0
AND 
LEN(TopicReply.Content) < 50
AND
TopicReply.CreateTime BETWEEN Topic.CreateTime AND  DATEADD(HOUR,48,Topic.CreateTime)
GROUP BY Circle.Id,Circle.Name
--SELECT * FROM #T16
----------插入统计表
		UPDATE  StaticData SET MasterReplyCountInvalid = (SELECT c FROM #T16 WHERE ID = StaticData.CircleID)
		WHERE EXISTS(SELECT 1 FROM #T16 WHERE ID = StaticData.CircleID)
-- 		SELECT * FROM StaticData



--DROP TABLE #userids
--DROP TABLE #LOGS
SELECT Id into #userids FROM  [ISCHOOLUSER].[DBO].userInfo WHERE mobile LIKE '12%'
SELECT [TIME],USERID INTO #LOGS  FROM  [10.1.0.16].[ISCHOOLLOGS].[DBO].[SxbLogs] WHERE [SxbLogs].[PLATFORM]=2  AND [TIME]>@btime AND [TIME]<@etime
--求得留存用户数
SELECT Circle.Id,Circle.Name,COUNT(CircleFollower.UserId) C INTO #T3 FROM iSchoolTopicCircle.dbo.CircleFollower
JOIN iSchoolTopicCircle.dbo.Circle ON CircleFollower.CircleId = Circle.Id
WHERE 
CircleFollower.Time between @btime and @etime
AND
CircleFollower.UserId NOT IN (SELECT ID FROM #userids)
AND 
EXISTS (
SELECT 1 FROM #LOGS WHERE  [#LOGS].UserId=CONVERT(varchar(50),CircleFollower.UserId) AND [#LOGS].[Time]  BETWEEN DATEADD(HOUR,12,CircleFollower.[Time] )AND DATEADD(HOUR,168,CircleFollower.[Time] ) 
)
GROUP BY Circle.Id,Circle.Name
-- SELECT * FROM #T3
----------插入统计表
		UPDATE  iSchoolTopicCircle.[dbo].StaticData SET STAYUSERCOUNT = (SELECT c FROM #T3 WHERE ID = StaticData.CircleID)
		WHERE EXISTS(SELECT 1 FROM #T3 WHERE ID = StaticData.CircleID)
-- 		SELECT * FROM iSchoolTopicCircle.[dbo].StaticData


";
            await _dbContext.ExecuteAsync(stastic, new { btime, etime }, null, 3600);
            string querySql = @"
USE iSchoolTopicCircle
SELECT 
CircleID 话题圈ID,
CircleName 话题圈名称,
NewUserCount 新用户数,
SubscribeUserCount 关注话题圈用户数,
StayUserCount 留存用户数,
(UserLikeCount - UserLikeCountReal) 灌水用户点赞数,
UserLikeCountReal 真实用户点赞数,
(UserCollectCount - UserCollectCountReal) 灌水用户收藏数,
UserCollectCountReal 真实用户收藏数,
UserShareCount 用户分享数,
UserShareCount 用户分享数,
(UserQACount - UserQACountReal) 灌水用户提问数,
UserQACountReal 真实用户提问数,
(UserTopicCount - UserTopicCountReal) 灌水用户发帖数,
UserTopicCountReal 真实用户发帖数,
(UserCommentCount-UserCommentCountReal) 灌水用户评论数,
UserCommentCountReal 真实用户评论数,
MasterTopicCount 圈主发帖数,
MasterReplyCountValid 圈主有效回复数,
MasterReplyCountInvalid 圈主无效回复数
FROM STATICDATA";
            return await _dbContext.QueryAsync(querySql);

        }
    }
}
