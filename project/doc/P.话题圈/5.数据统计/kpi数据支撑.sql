USE iSchoolTopicCircle
DELETE StaticData
INSERT StaticData(CircleID,CircleName) SELECT Id,NAME FROM Circle
SELECT * FROM StaticData
--================================================统计3.0==================================================================--
declare @btime datetime='2020-11-16 00:00:00' 
declare @etime datetime = '2020-11-22 23:59:59'
--拿出灌水用户
USE iSchoolUser
SELECT * INTO #GUANSHUIUSER FROM userInfo WHERE mobile LIKE '12%'
--SELECT * FROM #GUANSHUIUSER

USE iSchoolTopicCircle
SELECT * INTO #TARGETCIRCLE FROM Circle
--SELECT * FROM #TARGETCIRCLE
--INSERT INTO StaticData(CircleID,CircleName)  (select Circle.Id,Circle.Name from Circle)
--SELECT * FROM StaticData

--关注话题圈的新用户列表中
USE iSchoolTopicCircle
SELECT UserId,MIN(Time) T INTO #NEWUSER FROM CircleFollower WHERE Time>'2020-11-16'  GROUP BY UserId
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
		SELECT * FROM STATICDATA
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
		SELECT * FROM STATICDATA



----求得留存用户数
--SELECT Circle.Id,Circle.Name,COUNT(CircleFollower.UserId) C INTO #T3 FROM [10.1.0.15].iSchoolTopicCircle.dbo.CircleFollower
--JOIN [10.1.0.15].iSchoolTopicCircle.dbo.Circle ON CircleFollower.CircleId = Circle.Id
--WHERE 
--CircleFollower.Time between @btime and @etime
--AND
--CircleFollower.UserId NOT IN ( SELECT Id FROM [10.1.0.15].[ISCHOOLUSER].[DBO].userInfo WHERE mobile LIKE '12%')
--AND 
--EXISTS (
--SELECT 1 FROM [ISCHOOLLOGS].[DBO].[SxbLogs]  WHERE  [SxbLogs].UserId=CONVERT(varchar(50),CircleFollower.UserId) AND [SxbLogs].[PLATFORM]=2 AND [SxbLogs].[Time]  BETWEEN DATEADD(HOUR,12,CircleFollower.[Time] )AND DATEADD(HOUR,168,CircleFollower.[Time] ) 
--)
--GROUP BY Circle.Id,Circle.Name
--SELECT * FROM #T3
------------插入统计表
--		UPDATE  [10.1.0.15].iSchoolTopicCircle.[dbo].StaticData SET STAYUSERCOUNT = (SELECT c FROM #T3 WHERE ID = StaticData.CircleID)
--		WHERE EXISTS(SELECT 1 FROM #T3 WHERE ID = StaticData.CircleID)
--		SELECT * FROM [10.1.0.15].iSchoolTopicCircle.[dbo].StaticData



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
		SELECT * FROM StaticData

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
		SELECT * FROM StaticData


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
		SELECT * FROM StaticData



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
		SELECT * FROM StaticData

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
		SELECT * FROM StaticData

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
		SELECT * FROM StaticData

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
		SELECT * FROM StaticData



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
		SELECT * FROM StaticData


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
		SELECT * FROM StaticData



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
		SELECT * FROM StaticData



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
		SELECT * FROM StaticData



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
		SELECT * FROM StaticData

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
		SELECT * FROM StaticData




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
FROM STATICDATA



