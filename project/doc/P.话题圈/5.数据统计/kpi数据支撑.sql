USE iSchoolTopicCircle
DELETE StaticData
INSERT StaticData(CircleID,CircleName) SELECT Id,NAME FROM Circle
SELECT * FROM StaticData
--================================================ͳ��3.0==================================================================--
declare @btime datetime='2020-11-16 00:00:00' 
declare @etime datetime = '2020-11-22 23:59:59'
--�ó���ˮ�û�
USE iSchoolUser
SELECT * INTO #GUANSHUIUSER FROM userInfo WHERE mobile LIKE '12%'
--SELECT * FROM #GUANSHUIUSER

USE iSchoolTopicCircle
SELECT * INTO #TARGETCIRCLE FROM Circle
--SELECT * FROM #TARGETCIRCLE
--INSERT INTO StaticData(CircleID,CircleName)  (select Circle.Id,Circle.Name from Circle)
--SELECT * FROM StaticData

--��ע����Ȧ�����û��б���
USE iSchoolTopicCircle
SELECT UserId,MIN(Time) T INTO #NEWUSER FROM CircleFollower WHERE Time>'2020-11-16'  GROUP BY UserId
--����¹�ע����Ȧ���û�����
SELECT * into #NewCircleFollowers FROM CircleFollower
WHERE EXISTS(SELECT 1 FROM #NEWUSER WHERE CircleFollower.UserId=#NEWUSER.UserId AND CircleFollower.[Time]=#NEWUSER.T)
--SELECT * FROM #NewCircleFollowers

--��ù�ע����ŵ��û�����
USE iSchoolUser
 SELECT openid_weixin.openID,valid,appName,userID,SubscribeTime into #OPENIDS FROM openid_weixin 
 JOIN WXUSERINFO ON WXUSERINFO.OPENID=openid_weixin.openID
 --SELECT * FROM #OPENIDS

--����¹�ע�������Ϊ����Ȧ�¹�ע�û����û�����
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

----------����ͳ�Ʊ�
	    USE iSchoolTopicCircle
		UPDATE StaticData SET NewUserCount = (SELECT c FROM #T1 WHERE CircleID = StaticData.CircleID)
		WHERE EXISTS(SELECT 1 FROM #T1 WHERE CircleID = StaticData.CircleID)
		SELECT * FROM STATICDATA
--��þɹ�ע����ţ��¹�ע����Ȧ���û�����
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
----------����ͳ�Ʊ�
	    USE iSchoolTopicCircle
		UPDATE StaticData SET SubscribeUserCount = (SELECT c FROM #T2 WHERE CircleID = StaticData.CircleID)
		WHERE EXISTS(SELECT 1 FROM #T2 WHERE CircleID = StaticData.CircleID)
		SELECT * FROM STATICDATA



----��������û���
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
------------����ͳ�Ʊ�
--		UPDATE  [10.1.0.15].iSchoolTopicCircle.[dbo].StaticData SET STAYUSERCOUNT = (SELECT c FROM #T3 WHERE ID = StaticData.CircleID)
--		WHERE EXISTS(SELECT 1 FROM #T3 WHERE ID = StaticData.CircleID)
--		SELECT * FROM [10.1.0.15].iSchoolTopicCircle.[dbo].StaticData



--���Ȧ���û������ܼ���
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
----------����ͳ�Ʊ�
		UPDATE  StaticData SET UserTopicCount = (SELECT c FROM #T4 WHERE ID = StaticData.CircleID)
		WHERE EXISTS(SELECT 1 FROM #T4 WHERE ID = StaticData.CircleID)
		SELECT * FROM StaticData

--���Ȧ����ʵ�û������ܼ���
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
----------����ͳ�Ʊ�
		UPDATE  StaticData SET UserTopicCountReal = (SELECT c FROM #T5 WHERE ID = StaticData.CircleID)
		WHERE EXISTS(SELECT 1 FROM #T5 WHERE ID = StaticData.CircleID)
		SELECT * FROM StaticData


--���Ȧ���û����޼���
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
----------����ͳ�Ʊ�
		UPDATE  StaticData SET UserLikeCount = (SELECT c FROM #T6 WHERE ID = StaticData.CircleID)
		WHERE EXISTS(SELECT 1 FROM #T6 WHERE ID = StaticData.CircleID)
		SELECT * FROM StaticData



--���Ȧ����ʵ�û����޼���
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
----------����ͳ�Ʊ�
		UPDATE  StaticData SET UserLikeCountReal = (SELECT c FROM #T7 WHERE ID = StaticData.CircleID)
		WHERE EXISTS(SELECT 1 FROM #T7 WHERE ID = StaticData.CircleID)
		SELECT * FROM StaticData

--���Ȧ���û��ղػ��⼯��
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
----------����ͳ�Ʊ�
		UPDATE  StaticData SET UserCollectCount = (SELECT c FROM #T8 WHERE ID = StaticData.CircleID)
		WHERE EXISTS(SELECT 1 FROM #T8 WHERE ID = StaticData.CircleID)
		SELECT * FROM StaticData

--���Ȧ����ʵ�û��ղػ��⼯��
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
----------����ͳ�Ʊ�
		UPDATE  StaticData SET UserCollectCountReal = (SELECT c FROM #T9 WHERE ID = StaticData.CircleID)
		WHERE EXISTS(SELECT 1 FROM #T9 WHERE ID = StaticData.CircleID)
		SELECT * FROM StaticData

--���Ȧ���û����ʼ���
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
----------����ͳ�Ʊ�
		UPDATE  StaticData SET UserQACount = (SELECT c FROM #T10 WHERE ID = StaticData.CircleID)
		WHERE EXISTS(SELECT 1 FROM #T10 WHERE ID = StaticData.CircleID)
		SELECT * FROM StaticData



--���Ȧ����ʵ�û����ʼ���
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
----------����ͳ�Ʊ�
		UPDATE  StaticData SET UserQACountReal = (SELECT c FROM #T11 WHERE ID = StaticData.CircleID)
		WHERE EXISTS(SELECT 1 FROM #T11 WHERE ID = StaticData.CircleID)
		SELECT * FROM StaticData


--���Ȧ���û�������
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
----------����ͳ�Ʊ�
		UPDATE  StaticData SET UserCommentCount = (SELECT c FROM #T12 WHERE ID = StaticData.CircleID)
		WHERE EXISTS(SELECT 1 FROM #T12 WHERE ID = StaticData.CircleID)
		SELECT * FROM StaticData



--���Ȧ����ʵ�û�������
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
----------����ͳ�Ʊ�
		UPDATE  StaticData SET UserCommentCountReal = (SELECT c FROM #T13 WHERE ID = StaticData.CircleID)
		WHERE EXISTS(SELECT 1 FROM #T13 WHERE ID = StaticData.CircleID)
		SELECT * FROM StaticData



--Ȧ��Ȧ����������
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
----------����ͳ�Ʊ�
		UPDATE  StaticData SET MasterTopicCount = (SELECT c FROM #T14 WHERE ID = StaticData.CircleID)
		WHERE EXISTS(SELECT 1 FROM #T14 WHERE ID = StaticData.CircleID)
		SELECT * FROM StaticData



--Ȧ����Ч�ظ���
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
----------����ͳ�Ʊ�
		UPDATE  StaticData SET MasterReplyCountValid = (SELECT c FROM #T15 WHERE ID = StaticData.CircleID)
		WHERE EXISTS(SELECT 1 FROM #T15 WHERE ID = StaticData.CircleID)
		SELECT * FROM StaticData

--Ȧ����Ч�ظ���
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
----------����ͳ�Ʊ�
		UPDATE  StaticData SET MasterReplyCountInvalid = (SELECT c FROM #T16 WHERE ID = StaticData.CircleID)
		WHERE EXISTS(SELECT 1 FROM #T16 WHERE ID = StaticData.CircleID)
		SELECT * FROM StaticData




USE iSchoolTopicCircle
SELECT 
CircleID ����ȦID,
CircleName ����Ȧ����,
NewUserCount ���û���,
SubscribeUserCount ��ע����Ȧ�û���,
StayUserCount �����û���,
(UserLikeCount - UserLikeCountReal) ��ˮ�û�������,
UserLikeCountReal ��ʵ�û�������,
(UserCollectCount - UserCollectCountReal) ��ˮ�û��ղ���,
UserCollectCountReal ��ʵ�û��ղ���,
UserShareCount �û�������,
UserShareCount �û�������,
(UserQACount - UserQACountReal) ��ˮ�û�������,
UserQACountReal ��ʵ�û�������,
(UserTopicCount - UserTopicCountReal) ��ˮ�û�������,
UserTopicCountReal ��ʵ�û�������,
(UserCommentCount-UserCommentCountReal) ��ˮ�û�������,
UserCommentCountReal ��ʵ�û�������,
MasterTopicCount Ȧ��������,
MasterReplyCountValid Ȧ����Ч�ظ���,
MasterReplyCountInvalid Ȧ����Ч�ظ���
FROM STATICDATA



