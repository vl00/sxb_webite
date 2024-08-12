DROP TABLE #userids
DROP TABLE #LOGS
declare @btime datetime='2021-1-4 00:00:00' 
declare @etime datetime = '2021-1-10 23:59:59'
SELECT Id into #userids FROM [10.1.0.15].[ISCHOOLUSER].[DBO].userInfo WHERE mobile LIKE '12%'
SELECT [TIME],USERID INTO #LOGS  FROM  [ISCHOOLLOGS].[DBO].[SxbLogs] WHERE [SxbLogs].[PLATFORM]=2  AND [TIME]>@btime AND [TIME]<@etime
--求得留存用户数
declare @btime datetime='2021-1-4 00:00:00' 
declare @etime datetime = '2021-1-10 23:59:59'
SELECT Circle.Id,Circle.Name,COUNT(CircleFollower.UserId) C INTO #T3 FROM [10.1.0.15].iSchoolTopicCircle.dbo.CircleFollower
JOIN [10.1.0.15].iSchoolTopicCircle.dbo.Circle ON CircleFollower.CircleId = Circle.Id
WHERE 
CircleFollower.Time between @btime and @etime
AND
CircleFollower.UserId NOT IN (SELECT ID FROM #userids)
AND 
EXISTS (
SELECT 1 FROM #LOGS WHERE  [#LOGS].UserId=CONVERT(varchar(50),CircleFollower.UserId) AND [#LOGS].[Time]  BETWEEN DATEADD(HOUR,12,CircleFollower.[Time] )AND DATEADD(HOUR,168,CircleFollower.[Time] ) 
)
GROUP BY Circle.Id,Circle.Name
SELECT * FROM #T3
----------插入统计表
		UPDATE  [10.1.0.15].iSchoolTopicCircle.[dbo].StaticData SET STAYUSERCOUNT = (SELECT c FROM #T3 WHERE ID = StaticData.CircleID)
		WHERE EXISTS(SELECT 1 FROM #T3 WHERE ID = StaticData.CircleID)
		SELECT * FROM [10.1.0.15].iSchoolTopicCircle.[dbo].StaticData

