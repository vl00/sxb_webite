using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PMS.School.Domain.Entities;
using System.Collections.Generic;

namespace PMS.School.Domain.IRepositories
{
    public interface ISchoolScoreRepository
    {
        SchoolScore GetSchoolScore(Guid schoolId, Guid schoolSectionId);

        bool AddSchoolScore(SchoolScore schoolScore);

        bool UpdateSchoolScore(SchoolScore schoolScore);

        bool UpdateSchoolQuestionTotal(SchoolScore schoolScore);

        bool UpdateCommentTotal(Guid SchoolSectionId, DateTime lastCommentTime);

        bool UpdateQuestionTotal(Guid schoolSectionId, DateTime lastQuestionTime);


        /// <summary>
        /// 获取该学校的家长点评分数
        /// </summary>
        /// <param name="eid"></param>
        /// <returns></returns>
        decimal GetSchCommentAggScore(Guid eid);

        /// <summary>
        /// 获取区域平均分
        /// </summary>
        /// <returns></returns>
        Task<Dictionary<int, double>> GetAvgByAreaCode(int areaCode, string schFType = null);
        /// <summary>
        /// 获取区域平均分
        /// </summary>
        /// <returns></returns>
        Task<Dictionary<int, double>> GetAvgByCityCode(int cityCode, string schFType = null);
        /// <summary>
        /// 获取全国平均分
        /// </summary>
        /// <returns></returns>
        Task<Dictionary<int, double>> GetAvgScore(string schFType = null);
        /// <summary>
        /// 查询传入学校分数在指定城市的评分排名
        /// </summary>
        /// <returns></returns>
        Task<double> GetSchoolRankingInCity(int cityCode, double score, string schFType);
        /// <summary>
        /// 获取当前类型的分数在区域超过的百分比
        /// </summary>
        /// <returns></returns>
        Task<double> GetLowerPercent(string schFType, int areaCode, int indexID, double score);
        Task<IEnumerable<(Guid ExtId, double Score)>> GetExt22Scores(IEnumerable<Guid> extIds);
        Task<IEnumerable<KeyValuePair<Guid, int>>> GetAggScoreByEIDs(IEnumerable<Guid> eids);

    }
}
