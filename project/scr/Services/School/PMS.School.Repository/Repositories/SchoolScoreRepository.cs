using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using PMS.School.Domain.Entities;
using PMS.School.Domain.IRepositories;
using ProductManagement.Framework.MSSQLAccessor.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMS.School.Repository.Repositories
{
    public class SchoolScoreRepository : ISchoolScoreRepository
    {
        readonly ISchoolDataDBContext dbc;
        public SchoolScoreRepository(ISchoolDataDBContext dbc)
        {
            this.dbc = dbc;
        }

        public SchoolScore GetSchoolScore(Guid schoolId, Guid schoolSectionId)
        {
            string sql = @"SELECT TOP 1 [SchoolSectionId]
                          ,[SchoolId]
                          ,[AggScore]
                          ,[CommentCount]
                          ,[AttendCommentCount]
                          ,[TeachScore]
                          ,[HardScore]
                          ,[EnvirScore]
                          ,[ManageScore]
                          ,[LifeScore]
                          ,[CreateTime]
                          ,[UpdateTime]
                          ,[LastCommentTime]
                          ,[QuestionCount]
                          ,[LastQuestionTime]
                      FROM [iSchoolProduct].[dbo].[SchoolScores] where [SchoolSectionId] = @schoolSectionId;";


            var result = dbc.Query<SchoolScore>(sql, new { schoolId, schoolSectionId }).FirstOrDefault();
            return result;
        }

        public bool AddSchoolScore(SchoolScore schoolScore)
        {
            string sql = @"INSERT INTO [iSchoolProduct].[dbo].[SchoolScores]
                           ([SchoolSectionId]
                           ,[SchoolId]
                           ,[AggScore]
                           ,[CommentCount]
                           ,[AttendCommentCount]
                           ,[TeachScore]
                           ,[HardScore]
                           ,[EnvirScore]
                           ,[ManageScore]
                           ,[LifeScore]
                           ,[CreateTime]
                           ,[UpdateTime]
                           ,[LastCommentTime]
                           ,[QuestionCount]
                           ,[LastQuestionTime])
                     VALUES
                           (@SchoolSectionId,@SchoolId,@AggScore,@CommentCount,@AttendCommentCount,
                            @TeachScore, @HardScore,  @EnvirScore, @ManageScore, @LifeScore,
                            @CreateTime, @UpdateTime,@LastCommentTime, @QuestionCount, @LastQuestionTime);";


            return dbc.Execute(sql, new
            {
                schoolScore.SchoolSectionId,
                schoolScore.SchoolId,
                schoolScore.AggScore,
                schoolScore.CommentCount,
                schoolScore.AttendCommentCount,
                schoolScore.TeachScore,
                schoolScore.HardScore,
                schoolScore.EnvirScore,
                schoolScore.ManageScore,
                schoolScore.LifeScore,
                CreateTime = DateTime.Now,
                UpdateTime = DateTime.Now,
                schoolScore.LastCommentTime,
                schoolScore.QuestionCount,
                schoolScore.LastQuestionTime
            }) > 0;
        }

        public bool UpdateQuestionTotal(Guid schoolSectionId, DateTime lastQuestionTime)
        {
            string sql = "update [iSchoolProduct].[dbo].[SchoolScores] set QuestionCount += 1,LastQuestionTime = @lastQuestionTime  where SchoolSectionId = @schoolSectionId";

            return dbc.Execute(sql, new { lastQuestionTime, schoolSectionId }) > 0;
        }

        public bool UpdateCommentTotal(Guid SchoolSectionId, DateTime lastCommentTime)
        {
            string sql = "update [iSchoolProduct].[dbo].[SchoolScores] set CommentCount+=1,LastCommentTime = @lastCommentTime where SchoolSectionId = @SchoolSectionId";

            return dbc.Execute(sql, new { SchoolSectionId, lastCommentTime }) > 0;
        }

        public bool UpdateSchoolScore(SchoolScore schoolScore)
        {
            string sql = @"update [iSchoolProduct].[dbo].[SchoolScores] Set 
            AggScore =  @aggScore,
            CommentCount = @commentCount,
            TeachScore = @teachScore, 
            HardScore = @hardScore, 
            EnvirScore @envirScore, 
            ManageScore = @manageScore, 
            LifeScore = @lifeScore,
            AttendCommentCount = @attendCount,
            LastCommentTime = @lastCommentTime
            where SchoolId = @schoolId and SchoolSectionId = @schoolSectionId and LastCommentTime < @lastCommentTime;";

            return dbc.Execute(sql, new
            {
                schoolSectionId = schoolScore.SchoolSectionId,
                schoolId = schoolScore.SchoolId,
                commentCount = schoolScore.CommentCount,
                aggScore = schoolScore.AggScore,
                teachScore = schoolScore.TeachScore,
                hardScore = schoolScore.HardScore,
                envirScore = schoolScore.EnvirScore,
                manageScore = schoolScore.ManageScore,
                lifeScore = schoolScore.LifeScore,
                attendCount = schoolScore.AttendCommentCount,
                lastCommentTime = schoolScore.LastCommentTime
            }) > 0;
        }


        public bool UpdateSchoolQuestionTotal(SchoolScore schoolScore)
        {
            string sql = @"update [iSchoolProduct].[dbo].[SchoolScores] Set 
             QuestionCount = @questionCount,
             LastQuestionTime = @lastQuestionTime
             where SchoolId = @schoolId and SchoolSectionId = @schoolSectionId and LastQuestionTime < @lastQuestionTime;";

            return dbc.Execute(sql, new
            {
                schoolSectionId = schoolScore.SchoolSectionId,
                schoolId = schoolScore.SchoolId,
                lastQuestionTime = schoolScore.LastQuestionTime,
                questionCount = schoolScore.QuestionCount
            }) > 0;
        }


        public decimal GetSchCommentAggScore(Guid eid)
        {
            string sql = "select AggScore from [iSchoolProduct].[dbo].[SchoolScores] where SchoolSectionId = @eid";
            return dbc.Query<decimal>(sql, new { eid }).FirstOrDefault();
        }

        public async Task<Dictionary<int, double>> GetAvgByAreaCode(int areaCode, string schFType = null)
        {
            var str_SQL = $@"SELECT
	                            s.indexid as [Key],
	                            AVG(s.score) as [Value]
                            FROM
	                            Score AS s
	                            LEFT JOIN OnlineSchoolExtContent AS osec ON osec.eid = s.eid 
                            WHERE
	                            osec.area = @areaCode 
                            GROUP BY
	                            s.indexid";
            if (!string.IsNullOrWhiteSpace(schFType))
            {
                str_SQL = $@"SELECT
	                            s.indexid AS [Key],
	                            AVG(s.score) AS [Value] 
                            FROM
	                            Score AS s
	                            LEFT JOIN OnlineSchoolExtContent AS osec ON osec.eid = s.eid
	                            LEFT JOIN OnlineSchoolExtension AS ose ON ose.id = s.eid 
                            WHERE
	                            osec.area = @areaCode
	                            AND ose.SchFtype = @schFType
                            GROUP BY
	                            s.indexid";
            }

            var finds = await dbc.QueryAsync<(int, double)>(str_SQL, new { areaCode, schFType });
            if (finds?.Any() == true)
            {
                return finds.ToDictionary(k => k.Item1, v => v.Item2);
            }
            return null;
        }
        public async Task<Dictionary<int, double>> GetAvgByCityCode(int CityCode, string schFType = null)
        {
            var str_SQL = $@"SELECT
	                            s.indexid as [Key],
	                            AVG(s.score) as [Value]
                            FROM
	                            Score AS s
	                            LEFT JOIN OnlineSchoolExtContent AS osec ON osec.eid = s.eid 
                            WHERE
	                            osec.area = @areaCode 
                            GROUP BY
	                            s.indexid";
            if (!string.IsNullOrWhiteSpace(schFType))
            {
                str_SQL = $@"SELECT
	                            s.indexid AS [Key],
	                            AVG(s.score) AS [Value] 
                            FROM
	                            Score AS s
	                            LEFT JOIN OnlineSchoolExtContent AS osec ON osec.eid = s.eid
	                            LEFT JOIN OnlineSchoolExtension AS ose ON ose.id = s.eid 
                            WHERE
	                            osec.city = @CityCode
	                            AND ose.SchFtype = @schFType
                            GROUP BY
	                            s.indexid";
            }

            var finds = await dbc.QueryAsync<(int, double)>(str_SQL, new { CityCode, schFType });
            if (finds?.Any() == true)
            {
                return finds.ToDictionary(k => k.Item1, v => v.Item2);
            }
            return null;
        }

        public async Task<IEnumerable<(Guid ExtId, double Score)>> GetExt22Scores(IEnumerable<Guid> extIds)
        {
            if (extIds?.Any() != true)
            {
                return Enumerable.Empty<(Guid, double)>();
            }
            var sql = $@" SELECT S.eid, S.score FROM Score S WHERE S.eid IN @extIds AND S.indexid=22";
            return await dbc.QueryAsync<(Guid, double)>(sql, new { extIds });
        }

        public async Task<Dictionary<int, double>> GetAvgScore(string schFType = null)
        {
            var str_Where = $"WHERE ose.SchFtype = @schFType";
            if (string.IsNullOrWhiteSpace(schFType)) str_Where = string.Empty;
            var str_SQL = $@"SELECT
	                            s.indexid AS [Key],
	                            AVG(s.score) AS [Value] 
                            FROM
	                            Score AS s
	                            LEFT JOIN OnlineSchoolExtension AS ose ON ose.id = s.eid 
                            {str_Where}   
                            GROUP BY
	                            s.indexid";

            var finds = await dbc.QueryAsync<(int, double)>(str_SQL, new { schFType });
            if (finds?.Any() == true)
            {
                return finds.ToDictionary(k => k.Item1, v => v.Item2);
            }
            return null;
        }

        public async Task<double> GetSchoolRankingInCity(int cityCode, double score, string schFType)
        {
            var str_AllCountSQL = $@"SELECT
	                                Count(s.id) 
                                FROM
	                                Score AS s
	                                LEFT JOIN OnlineSchoolExtContent AS osec ON osec.eid = s.eid 
	                                LEFT JOIN OnlineSchoolExtension as ose on ose.id = s.eid
                                WHERE
	                                s.indexid = 22 
	                                AND ose.SchFtype = @schFType
	                                AND osec.city = @cityCode;";
            var allCount = await dbc.QuerySingleAsync<int>(str_AllCountSQL, new { cityCode, schFType });
            var str_ScoreCountSQL = $@"SELECT
	                                        Count(s.id) 
                                        FROM
	                                        Score AS s
	                                        LEFT JOIN OnlineSchoolExtContent AS osec ON osec.eid = s.eid
	                                        LEFT JOIN OnlineSchoolExtension as ose on ose.id = s.eid
                                        WHERE
	                                        s.indexid = 22
	                                        AND ose.SchFtype = @schFType
	                                        AND osec.city = @cityCode
	                                        AND s.score >= @score";
            var scoreCount = await dbc.QuerySingleAsync<int>(str_ScoreCountSQL, new { cityCode, score, schFType });
            return (scoreCount / (allCount + 0.0));
        }

        public async Task<double> GetLowerPercent(string schFType, int areaCode, int indexID, double score)
        {
            var str_SQL = $@"DECLARE @allTotal DECIMAL;
                            SELECT
	                            @allTotal = COUNT ( s.id ) 
                            FROM
	                            Score AS s
	                            LEFT JOIN OnlineSchoolExtension AS ose ON ose.id = s.eid
	                            LEFT JOIN OnlineSchoolExtContent AS osec ON osec.eid = s.eid 
                            WHERE
	                            s.indexid = @indexID
	                            AND s.status = 1
	                            AND ose.SchFtype = @schFType
	                            AND osec.area = @areaCode;
                            DECLARE
	                            @targetTotal DECIMAL;
                            SELECT
	                            @targetTotal = COUNT ( s.id ) 
                            FROM
	                            Score AS s
	                            LEFT JOIN OnlineSchoolExtension AS ose ON ose.id = s.eid
	                            LEFT JOIN OnlineSchoolExtContent AS osec ON osec.eid = s.eid 
                            WHERE
	                            s.indexid = @indexID
	                            AND s.status = 1 
	                            AND ose.SchFtype = @schFType
	                            AND osec.area = @areaCode
	                            AND s.score < @score;
                            SELECT
	                            @targetTotal /@allTotal AS [Result];";
            return await dbc.QuerySingleAsync<double>(str_SQL, new { schFType, areaCode, indexID, score });
        }
        public async Task<IEnumerable<KeyValuePair<Guid, int>>> GetAggScoreByEIDs(IEnumerable<Guid> eids)
        {
            var str_SQL = $"SELECT Score,Eid FROM [dbo].[ScoreTotal] WHERE Eid IN @eids AND status = 1;";
            var finds = await dbc.QueryAsync<(int, Guid)>(str_SQL, new { eids });
            if (finds?.Any() == true)
            {
                return finds.Select(p => new KeyValuePair<Guid, int>(p.Item2, p.Item1));
            }
            return null;
        }
    }
}
