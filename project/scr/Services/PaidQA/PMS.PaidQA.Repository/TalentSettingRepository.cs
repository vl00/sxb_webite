using PMS.PaidQA.Domain.Dtos;
using PMS.PaidQA.Domain.Entities;
using ProductManagement.Framework.MSSQLAccessor.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMS.PaidQA.Repository
{
    public class TalentSettingRepository : Repository<TalentSetting, PaidQADBContext>, ITalentSettingRepository
    {
        PaidQADBContext _paidQADBContext;
        public TalentSettingRepository(PaidQADBContext dBContext) : base(dBContext)
        {
            _paidQADBContext = dBContext;
        }

        public async Task<IEnumerable<LevelType>> GetAllTalentLevels()
        {
            var str_SQL = "Select * from LevelType WHERE ISDEL=0 ORDER BY Sort";
            return await _paidQADBContext.QueryAsync<LevelType>(str_SQL, new { });
        }

        public async Task<string> GetLevelName(Guid talentUserID)
        {
            var str_SQL = $@"SELECT
	                            lt.Name 
                            FROM
	                            levelType AS lt
	                            LEFT JOIN TalentSetting AS ts ON ts.TalentLevelTypeID = lt.ID 
                            WHERE
	                            ts.TalentUserID = @talentUserID";
            return await _paidQADBContext.QuerySingleAsync<string>(str_SQL, new { talentUserID });
        }

        public async Task<IEnumerable<TalentSetting>> GetRandomTalents(int count, IEnumerable<Guid> notInTalentIDs = null)
        {
            var str_Where = String.Empty;

            if (notInTalentIDs?.Any() == true)
            {
                str_Where = " AND TalentUserID NOT IN @notInTalentIDs";
            }
            var str_SQL = $"Select top {count} * from TalentSetting Where IsEnable = 1 {str_Where} ORDER BY NEWID()";
            return await _paidQADBContext.QueryAsync<TalentSetting>(str_SQL, new { notInTalentIDs });
        }

        public async Task<IEnumerable<KeyValuePair<Guid, int>>> GetSimilarTalentIDs(Guid talentID)
        {
            var str_SQL = $@"SELECT
	                            t.TalentUserID as [Key], SUM(t.tempCount) as [Value]
                            FROM
	                            (
	                            SELECT
		                            ts.TalentUserID, tr.RegionTypeID, COUNT (1) AS [tempCount] 
	                            FROM
		                            TalentSetting AS ts
		                            LEFT JOIN TalentRegion AS tr ON tr.UserID = ts.TalentUserID
	                            WHERE
		                             tr.RegionTypeID IN (SELECT RegionTypeID FROM TalentRegion WHERE UserID = @talentID) AND IsEnable = 1 
	                            GROUP BY
		                            ts.TalentUserID, tr.RegionTypeID 
	                            UNION ALL
	                            SELECT
		                            ts.TalentUserID, tg.GradeID, COUNT (1) 
	                            FROM
		                            TalentSetting AS ts 
		                            LEFT JOIN TalentGrade AS tg ON tg.TalentUserID = ts.TalentUserID 
	                            WHERE
		                            tg.GradeID IN (SELECT GradeID FROM TalentGrade WHERE TalentUserID = @talentID) AND IsEnable = 1 
	                            GROUP BY
		                            ts.TalentUserID, tg.GradeID 
	                            ) 
	                            as t
                                LEFT JOIN iSchoolUser.dbo.talent on talent.user_id = t.TalentUserID
	                        WHERE
                                talent.IsInternal = 1
                                AND t.TalentUserID != @talentID
                                AND t.TalentUserID Not IN (Select DISTINCT(AnswerID) from [Order] WHERE CreatorID = @talentID AND Status in (0,1,2,3))
                            GROUP BY 
                                t.TalentUserID
                            ORDER BY 
                                [Value] DESC";
            return await _paidQADBContext.QueryAsync<KeyValuePair<Guid, int>>(str_SQL, new { talentID });
        }

        public async Task<IEnumerable<KeyValuePair<Guid, int>>> PageSimilarTalentIDs(IEnumerable<Guid> regionTypeIDs, IEnumerable<Guid> gradeIDs, int pageIndex = 1, int pageSize = 10, Guid? excludeUserID = null)
        {
            var str_Count = "0";

            if (regionTypeIDs?.Any() == true)
            {
                str_Count = "(Select Count(1) from TalentRegion WHERE UserID = ts.TalentUserID AND RegionTypeID in @regionTypeIDs)";
            }
            if (gradeIDs?.Any() == true)
            {
                if (str_Count != "0") str_Count += "+";
                str_Count += "(Select Count(1) from TalentGrade WHERE TalentUserID = ts.TalentUserID AND GradeID in @gradeIDs)";
                if (str_Count.StartsWith('0')) str_Count.TrimStart('0');
            }
            var str_excludeUserID = excludeUserID.HasValue ? $" AND ts.TalentUserID != '{excludeUserID.Value}'" : "";
            var str_SQL = $@"SELECT
	                            ts.TalentUserID as [KEY],
	                            {str_Count}
	                            as [VALUE]
                            FROM
	                            TalentSetting AS ts
                            WHERE
	                            ts.IsEnable = 1 {str_excludeUserID} 
                            ORDER BY 
	                            [VALUE] DESC
                            OFFSET {--pageIndex * pageSize} ROWS FETCH NEXT {pageSize} ROWS ONLY;";
            return await _paidQADBContext.QueryAsync<KeyValuePair<Guid, int>>(str_SQL, new { regionTypeIDs, gradeIDs });
        }

        public async Task<IEnumerable<PageTalentIDDto>> PageTalentIDs(int pageIndex = 1, int pageSize = 10, Guid? gradeID = null, Guid? regionTypeID = null, Guid? levelID = null, int orderTypeID = 1
            , decimal minPrice = 0, decimal maxPrice = 0, string nickName = null, bool isInternal = false)
        {
            var str_Join = string.Empty;
            var str_Where = string.Empty;
            var offset = --pageIndex * pageSize;
            if (gradeID.HasValue)
            {
                str_Where += " AND tg.GradeID = @gradeID";
            }
            if (regionTypeID.HasValue)
            {
                str_Where += " AND tr.RegionTypeID = @regionTypeID";
            }
            if (levelID.HasValue)
            {
                str_Where += " AND ts.TalentLevelTypeID = @levelID";
            }
            if (minPrice > 0)
            {
                str_Where += " AND ts.Price >= @minPrice";
            }
            if (maxPrice > 0)
            {
                str_Where += " AND ts.Price <= @maxPrice";
            }
            if (!string.IsNullOrWhiteSpace(nickName))
            {
                str_Join += " Left Join iSchoolUser.dbo.UserInfo as ui on ui.id = ts.TalentUserID ";
                str_Where += $" AND ui.nickName like '%{nickName.Trim()}%'";
            }
            if (isInternal)
            {
                str_Join += " Left Join iSchoolUser.dbo.Talent as t on t.user_id = ts.TalentUserID ";
                str_Where += $" AND t.Isinternal = 1";
            }

            var str_OrderBy = $@"
                                TopSort,
                                OrderCount DESC,
                                LevelSort DESC,
	                            AvgScore DESC,
	                            ReplyPercent DESC,
                                TalentUserID";
            switch (orderTypeID)
            {
                case 1:
                    str_OrderBy = "ReplyPercent desc";
                    break;
                case 2:
                    str_OrderBy = "ts.Price";
                    break;
            }
            var str_SQL = $@"
                            SELECT AnswerID,COUNT(1) OrderCount INTO #ORDERCOUNTTEMP FROM [Order]
                            WHERE 
                            [Status] = 4
                            AND EXISTS(SELECT 1 FROM TalentSetting WHERE IsEnable=1 AND TalentSetting.TalentUserID = AnswerID)
                            GROUP BY AnswerID;
                            SELECT
	                            ts.TalentUserID,
	                            ts.Price,
	                            ISNULL(AVG(o.FirstReplyTimespan),2147483647) as [AvgReplyTimespan],
	                            ISNULL(lt.Sort,0) as [LevelSort],
	                            ISNULL(AVG(e.Score),1) AS [AvgScore],
                                ISNULL(OCT.OrderCount,0) AS [OrderCount],
	                            ISNULL(
		                            (SELECT COUNT(1)*100 / (SELECT NULLIF(COUNT(1),0) FROM [Order] WHERE AnswerID = ts.TalentUserID AND status = 4) FROM [Order] 
		                            WHERE
			                            FirstReplyTimespan <= 21600 
			                            AND Status = 4 
			                            AND AnswerID = ts.TalentUserID 
		                            ),0) AS [ReplyPercent] ,
                                ISNULL(ttop.Sort,2147483647)  TopSort
                            FROM
	                            (
                                  -- SELECT TalentSetting.* FROM TalentSetting
                                  -- WHERE EXISTS(SELECT 1 FROM TalentTop WHERE TalentTop.USERID=TalentSetting.TalentUserID)
                                  -- UNION 
                                  SELECT ts.* FROM TalentSetting as ts
	                              LEFT JOIN TalentRegion as tr on tr.UserID = ts.TalentUserID
	                              LEFT JOIN TalentGrade as tg on tg.TalentUserID = ts.TalentUserID
                                  {str_Join}
                                  WHERE ts.IsEnable = 1
                                  {str_Where}
                                ) AS ts
	                            LEFT JOIN LevelType AS lt ON lt.ID = ts.TalentLevelTypeID
                                LEFT JOIN [Order] AS o ON o.AnswerID = ts.TalentUserID
	                            LEFT JOIN Evaluate AS e ON e.OrderID = o.ID 
                                LEFT JOIN #ORDERCOUNTTEMP AS OCT ON OCT.AnswerID = ts.TalentUserID
                                LEFT JOIN TalentTop ttop ON ttop.UserId = ts.TalentUserID

                            GROUP BY
	                            ts.TalentUserID,
	                            ts.Price,
	                            lt.Sort,
                                OCT.OrderCount,
                                ttop.Sort
                            ORDER BY
	                            {str_OrderBy}
                            OFFSET {offset} ROWS FETCH NEXT {pageSize} ROWS ONLY";
            return await _paidQADBContext.QueryAsync<PageTalentIDDto>(str_SQL, new { gradeID, regionTypeID, levelID, minPrice, maxPrice, nickName });
        }
        public async Task<TalentRecordDto> GetTalentRecord(Guid userId)
        {
            string sql = @"
SELECT TS.*,TR.* FROM TalentSetting TS
LEFT JOIN TalentRecord TR ON TR.UserId  = TS.TalentUserID
WHERE 
TS.TalentUserID=@userId ";
         var result =  await  _paidQADBContext.QueryAsync<TalentSetting, TalentRecord, TalentRecordDto>(sql, (ts, tr) =>
            {
                return new TalentRecordDto()
                {
                    TalentLevelTypeID = ts.TalentLevelTypeID,
                    TalentUserID = ts.TalentUserID,
                    Price = ts.Price,
                    IsEnable = ts.IsEnable,
                    CreateTime = tr?.CreateTime,
                    TalentIntro = tr?.TalentIntro,
                    RegionDesc = tr?.RegionDesc,
                    JA_Covers = ts.JA_Covers
                };
            }, new { userId },splitOn:"userId");
            return result.FirstOrDefault();

        }

       

       




        public async Task<IEnumerable<(int Grade, Guid UserId)>> GradeUserIds()
        {
            var str_SQL = $@"
SELECT
	MIN(g.sort) AS Grade,
	MIN(ts.TalentUserID) AS UserId
FROM
	TalentSetting AS ts
	INNER JOIN TalentGrade as tg on tg.TalentUserID = ts.TalentUserID
	INNER JOIN Grade AS g ON g.ID = tg.GradeID
WHERE 	
	ts.IsEnable = 1
    -- AND tg.GradeID = @gradeID
GROUP BY
	tg.GradeID
                            ";
            return await _paidQADBContext.QueryAsync<(int, Guid)>(str_SQL, new{ });
        }

        public async Task<IEnumerable<TalentSetting>> GetBySchool(Guid schoolExtId)
        {
            string sql = @"SELECT * FROM TalentSetting
WHERE 
IsEnable=1
AND EXISTS(
SELECT 1 FROM iSchoolUser.dbo.talentSchoolExtension
JOIN iSchoolUser.dbo.talent ON talent.ID = talentSchoolExtension.talentId
WHERE TalentSetting.TalentUserID = talent.user_id AND talentSchoolExtension.eid = @eid
)";
            return await _paidQADBContext.QueryAsync<TalentSetting>(sql,new { eid = schoolExtId});
        }

        public async Task<Guid?> GetSchoolId(Guid talentUserId)
        {
            string sql = @" SELECT top 1  [talentSchoolExtension].eid FROM [iSchoolUser].[dbo].[talentSchoolExtension]
  JOIN  [iSchoolUser].[dbo].[talent] ON talent.id = talentSchoolExtension.talentId
  where talent.user_id= @userId";
            return await _paidQADBContext.ExecuteScalarAsync<Guid?>(sql, new { userId = talentUserId });
        }


    }
}
