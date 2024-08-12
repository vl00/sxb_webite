using Dapper;
using PMS.Infrastructure.Domain.Dtos;
using PMS.Infrastructure.Domain.Entities;
using PMS.Infrastructure.Domain.IRepositories;
using ProductManagement.Framework.MSSQLAccessor;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PMS.Infrastructure.Repository.Repository
{
    public class SystemEventLogRepository : Repository<SystemEventLog, JcDbContext>, ISystemEventLogRepository
    {
        private JcDbContext _dbContext;

        public SystemEventLogRepository(JcDbContext jcDbContext) : base(jcDbContext)
        {

            _dbContext = jcDbContext;
        }

        public async Task<IEnumerable<SystemEventLogExportDataDto>> ExportData(string plateform,string where = null, object param = null, string orderBy = null)
        {
            DynamicParameters parameters = new DynamicParameters();
            StringBuilder sql = new StringBuilder(@"
USE iSchoolData;
declare @citynametable table (id int,name varchar(200))
insert into @citynametable  SELECT K2.id,K1.name + K2.name name  FROM KeyValue K1
JOIN KeyValue K2 ON K1.id = K2.parentid
WHERE K1.[type]=1 AND K1.parentid=0

USE ISCHOOL
DELETE  SystemEventLog where isjson(Body)=0;
SELECT  
SystemEventLog.Id 
,AppName 
,AppVersion
,UserId
,USERINFO.nickname
,(CASE SystemEventLog.Client WHEN 1 THEN 'H5' WHEN 2 THEN 'PC' WHEN 3 THEN '小程序' WHEN 4 THEN 'App' ELSE '未知' END) Client
,Equipment
,(CASE  WHEN UserId = '00000000-0000-0000-0000-000000000000' THEN 0 ELSE 1 END) Login
,(CASE WHEN UserId!='00000000-0000-0000-0000-000000000000' AND IsTalent=1 THEN 1 ELSE 0 END) Talent
,System
,(case when Location is null then '广东广州' else (select name from @citynametable where id=Location) end) Location
,CONVERT(varchar(100), CreateTime, 111) Date
,CONVERT(varchar(100), CreateTime, 108) Time
,Event
,EventId
,Body
,jsn.[version]
FROM SystemEventLog
OUTER APPLY (
SELECT * FROM OPENJSON(body)
WITH (
    pagePath varchar(1000) '$.pagePath',
	[version] varchar(50) '$.version'
))as jsn
Left JOIN iSchoolUser.DBO.USERINFO ON SystemEventLog.USERID = USERINFO.ID
");
            parameters.AddDynamicParams(new { pagePath  = $"%//m.sxkid.com/{plateform}/%" });
            if (!string.IsNullOrEmpty(where))
            {
                sql.AppendFormat("  WHERE  ({0}) ", where);
                parameters.AddDynamicParams(param);
            }
            if (!string.IsNullOrEmpty(orderBy))
            {
                sql.AppendFormat(" Order By {0} ", orderBy);
            }

            return await _dbContext.QueryAsync<SystemEventLogExportDataDto>(sql.ToString(), parameters);

        }

        /// <summary>
        /// 获取指定时间之后访问过的url
        /// </summary>
        /// <param name="urls"></param>
        /// <param name="startTime"></param>
        /// <returns></returns>
        public async Task<IEnumerable<string>> GetHitUrls(Guid userId, string[] urls, DateTime startTime)
        {
            var sql = $@"
SELECT
	DISTINCT JSN.PagePath
FROM
	iSchool.dbo.SystemEventLog SEL
	OUTER APPLY(
		SELECT * FROM OPENJSON(BODY)
		WITH (
			pagePath varchar(1000) '$.pagePath'
		)
	) AS JSN
WHERE
	SEL.[Event] = 'PAGEVIEW'
	AND UserId = @userId
	AND SEL.CreateTime >= @startTime
	AND JSN.PagePath IN @urls
";
            return await _dbContext.QueryAsync<string>(sql, new { userId, urls, startTime });
        }

        /// <summary>
        /// 获取每日Pv Uv
        /// </summary>
        /// <param name="startWithUrl"></param>
        /// <param name="startTime"></param>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public async Task<IEnumerable<SystemEventLogPvUvDto>> GetDaysPvUv(string startWithUrl, DateTime? startTime, DateTime? endTime)
        {
            startWithUrl = startWithUrl.Trim() + "%";
            var startTimeSql = startTime == null ? "" : " AND CreateTime >= @startTime";
            var endTimeSql = endTime == null ? "" : " AND CreateTime < @endTime";
            var sql = $@"
SELECT  
	PagePath,
    MIN(Title) AS Title,
	CONVERT(varchar(10), CreateTime, 111) as Date,
	COUNT(DISTINCT userId) AS Uv,
	COUNT(1) AS Pv
	 -- json_value(SystemEventLog.Body,'$.version') 
FROM 
	SystemEventLog
	OUTER APPLY (
		SELECT * FROM OPENJSON(body)
		WITH (pagePath varchar(1000) '$.pagePath', Title varchar(1000) '$.title')
	)as jsn
WHERE
    [Event] = 'PAGEVIEW'
    {startTimeSql}
    {endTimeSql}
    AND pagePath like @startWithUrl
GROUP BY
	pagePath, CONVERT(varchar(10), CreateTime, 111)
";
            return await _dbContext.QueryAsync<SystemEventLogPvUvDto>(sql, new { startWithUrl, startTime, endTime });
        }
    }
}
