using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PMS.School.Application.ModelDto;
using PMS.School.Domain.Entities.Mongo;

namespace PMS.School.Application.IServices
{
    public interface ISchoolScoreService
    {
        SchoolScoreDto GetSchoolScore(Guid schoolId, Guid schoolSectionId);
        void SyncSchoolScore(SchoolScoreDto schoolScore);
        void SyncSchoolQuestionTotal(SchoolScoreDto schoolScore);

        bool UpdateCommentTotal(Guid SchoolSectionId, DateTime lastCommentTime);

        bool UpdateQuestionTotal(Guid schoolSectionId, DateTime lastQuestionTime);

        /// <summary>
        /// 获取该学校的家长点评分数
        /// </summary>
        /// <param name="eid"></param>
        /// <returns></returns>
        int GetSchCommentAggScore(Guid eid);

        /// <summary>
        /// 获取区域平均分
        /// </summary>
        /// <param name="areaCode">区域代号</param>
        /// <returns></returns>
        Task<Dictionary<int, double>> GetAvgScoreByAreaCode(int areaCode, string schFType = null);
        /// <summary>
        /// 获取市区平均分
        /// </summary>
        /// <param name="cityCode">市区代号</param>
        Task<Dictionary<int, double>> GetAvgScoreByCityCode(int cityCode, string schFType = null);
        /// <summary>
        /// 获取全国平均分
        /// </summary>
        /// <returns></returns>
        Task<Dictionary<int, double>> GetAvgScore(string schFType = null);


        /// <summary>
        /// 获取学校在城市中的排名百分比
        /// <para>比如 排名前15%</para>
        /// </summary>
        /// <param name="cityCode">城市代码</param>
        /// <param name="score">分数</param>
        /// <param name="schFType">学校类型</param>
        /// <returns></returns>
        Task<double> GetSchoolRankingInCity(int cityCode, double score, string schFType);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cityCode"></param>
        /// <param name="schFType"></param>
        /// <returns></returns>
        Task<int> GetSchoolCountByCityAndSchFType(int cityCode, string schFType);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cityCode"></param>
        /// <param name="schFType"></param>
        /// <returns></returns>
        Task<int> GetSchoolCountByAreaAndSchFType(int areaCode, string schFType);

        /// <summary>
        /// 获取指定区域指定分数段的学校数量
        /// </summary>
        /// <returns></returns>
        Task<int> GetSchoolCount(string schFtype, int areaCode, int minScore, int maxScore);

        /// <summary>
        /// 获取指定区域大于某分数的学校数量
        /// </summary>
        /// <returns></returns>
        Task<int> GetSchoolCount(string schFtype, int areaCode, int maxScore);

        /// <summary>
        /// 获取当前类型的分数在区域超过的百分比
        /// </summary>
        /// <returns></returns>
        Task<double> GetLowerPercent(string schFType, int areaCode, int indexID, double score);

        /// <summary>
        /// 批量获取学校评分
        /// </summary>
        /// <param name="eids">分部IDs</param>
        /// <returns></returns>
        Task<IEnumerable<KeyValuePair<Guid, int>>> GetSchoolAggScores(IEnumerable<Guid> eids);
    }
}
