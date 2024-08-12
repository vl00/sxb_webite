using PMS.Infrastructure.Domain.IRepositories;
using ProductManagement.Framework.MSSQLAccessor.DBContext;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PMS.Infrastructure.Repository.Repository
{
    public  class DataExportRepository :IDataExportRepository
    {
        PaidQADBContext _dbcontext;
        public DataExportRepository(PaidQADBContext paidQADBContext)
        {
            _dbcontext = paidQADBContext;
        }


		public async Task<IEnumerable<dynamic>> ExportPageRecords(DateTime btime, DateTime etime)
		{
			string sql = @"
DECLARE @PVS TABLE(
	NAME VARCHAR(1000),
	PV INT
)
DECLARE @UVS TABLE(
	NAME VARCHAR(1000),
	UV INT
)
DECLARE @PAGES TABLE (
	PageName VARCHAR(50),
	Path VARCHAR(1000)
)
insert into @PAGES VALUES('推荐咨询','https://m.sxkid.com/ask/home/%')
insert into @PAGES VALUES('推荐咨询-热门咨询','https://m.sxkid.com/ask/hot/%')
insert into @PAGES VALUES('咨询主页','https://m.sxkid.com/ask/expert/detail%')
insert into @PAGES VALUES('问专家','https://m.sxkid.com/ask/expert/list%')
insert into @PAGES VALUES('发起咨询','https://m.sxkid.com/ask/refer%')

USE iSchool
INSERT INTO @PVS 
SELECT  P.PageName Name,COUNT(1) PV  FROM SystemEventLog 
	OUTER APPLY (
	SELECT * FROM OPENJSON(body)
	WITH (
			fw VARCHAR(50) '$.fw',
			pagePath varchar(1000) '$.pagePath',
			title VARCHAR(1000) '$.title'
	))as jsn
	JOIN @PAGES P ON pagePath like P.Path
WHERE 
Event = 'PAGEVIEW'
AND SystemEventLog.CreateTime BETWEEN @BTIME AND @ETIME
GROUP BY P.PageName

USE iSchool
DECLARE @UVTABLE TABLE(PAGENAME VARCHAR(1000),USERID VARCHAR(300))
INSERT INTO @UVTABLE  SELECT  P.PageName,SystemEventLog.USERID  FROM SystemEventLog 
OUTER APPLY (
SELECT * FROM OPENJSON(body)
WITH (
		fw VARCHAR(50) '$.fw',
    pagePath varchar(1000) '$.pagePath',
		title VARCHAR(1000) '$.title'
))as jsn
JOIN @PAGES P ON pagePath like P.Path
WHERE 
Event = 'PAGEVIEW'
AND USERID IS NOT NULL
AND SystemEventLog.CreateTime BETWEEN @BTIME AND @ETIME
GROUP BY P.PageName,SystemEventLog.USERID

INSERT INTO @UVS SELECT PAGENAME,COUNT(1) UV  FROM @UVTABLE GROUP BY PAGENAME


DECLARE @PRIMARYTABLE table(NAME VARCHAR(1000))

INSERT INTO @PRIMARYTABLE
SELECT NAME FROM (
SELECT NAME FROM @PVS
UNION 
SELECT NAME FROM @UVS
) PRIMARYTABLE


SELECT A.NAME 页面名称,d.Path 页面链接,B.PV ,C.UV FROM @PRIMARYTABLE A
LEFT JOIN @PVS B ON B.NAME = A.NAME
LEFT JOIN @UVS C ON C.NAME = A.NAME
LEFT JOIN @PAGES d  ON D.PageName = A.NAME


";
			return await _dbcontext.QueryAsync(sql, new { BTIME = btime, ETIME = etime }, commandTimeout: 3600);

		}

		public async Task<IEnumerable<dynamic>> ExportFWRecords(DateTime btime, DateTime etime)
		{
			string sql = @"
if object_id(N'tempdb.dbo.#TCHANEL',N'U') is not null
    DROP TABLE tempdb.dbo.#TCHANEL
if object_id(N'tempdb.dbo.#PVS',N'U') is not null
    DROP TABLE tempdb.dbo.#PVS
if object_id(N'tempdb.dbo.#UOS',N'U') is not null
    DROP TABLE tempdb.dbo.#UOS
if object_id(N'tempdb.dbo.#OS',N'U') is not null
    DROP TABLE tempdb.dbo.#OS
if object_id(N'tempdb.dbo.#OPS',N'U') is not null
    DROP TABLE tempdb.dbo.#OPS
		
USE iSchool
SELECT jsn.fw Name into #TCHANEL FROM SystemEventLog 
	OUTER APPLY (
	SELECT * FROM OPENJSON(body)
	WITH (
			fw VARCHAR(50) '$.fw',
			pagePath varchar(1000) '$.pagePath'
-- 			title VARCHAR(1000) '$.title'
	))as jsn
WHERE 
Event = 'PAGEVIEW'
AND SystemEventLog.CreateTime BETWEEN @BTIME AND @ETIME
AND jsn.pagePath like 'https://m.sxkid.com/ask%'
AND jsn.fw != ''
GROUP BY jsn.fw


--统计PV
USE iSchool
-- INSERT INTO @PVS 
SELECT jsn.fw Name,COUNT(1) Number  
into #PVS 
FROM SystemEventLog 
	OUTER APPLY (
	SELECT * FROM OPENJSON(body)
	WITH (
			fw VARCHAR(50) '$.fw'
-- 			pagePath varchar(1000) '$.pagePath',
-- 			title VARCHAR(1000) '$.title'
	))as jsn
WHERE 
Event = 'PAGEVIEW'
AND SystemEventLog.CreateTime BETWEEN @BTIME AND @ETIME
AND EXISTS(SELECT 1 FROM #TCHANEL C WHERE C.NAME = jsn.fw )
GROUP BY jsn.fw


--统计付费订单人数
use iSchoolPaidQA
-- INSERT INTO @UOS
SELECT ORIGINTYPE Name ,COUNT(1) Number 
into #UOS
FROM (
select ORIGINTYPE,CreatorID  from  [Order]
where
CREATETIME BETWEEN @BTIME AND @ETIME
AND Status > 0
AND PayAmount>0
AND EXISTS(SELECT 1 FROM #TCHANEL C WHERE ORIGINTYPE=C.NAME)
 GROUP BY ORIGINTYPE,CreatorID
) TEMP
GROUP BY TEMP.ORIGINTYPE

--统计付费订单数
use iSchoolPaidQA
-- INSERT INTO @OS 
select ORIGINTYPE Name,COUNT(1) Number
into #OS
from  [Order]
where
CREATETIME BETWEEN @BTIME AND @ETIME
AND Status > 0
AND PayAmount>0
AND EXISTS(SELECT 1 FROM #TCHANEL C WHERE ORIGINTYPE=C.NAME)
 GROUP BY ORIGINTYPE
 
 
--统计付费总金额
use iSchoolPaidQA
-- INSERT INTO @OS 
select ORIGINTYPE Name,SUM(PayAmount) Number
into #OPS
from  [Order]
where
CREATETIME BETWEEN @BTIME AND @ETIME
AND Status > 0
AND PayAmount>0
AND EXISTS(SELECT 1 FROM #TCHANEL C WHERE ORIGINTYPE=C.NAME)
 GROUP BY ORIGINTYPE

 
select a.name 渠道名称,b.number PV,c.number 付费人数,d.number 付费订单数 ,e.number 付费总金额 from #TCHANEL a
left join #pvs b on a.name = b.name 
left join #uos c on a.name = c.name 
left join #os d on a.name = d.name 
left join #ops e on a.name = e.name 


";
			return await _dbcontext.QueryAsync(sql, new { BTIME = btime, ETIME = etime }, commandTimeout: 3600);
		}


	}
}
