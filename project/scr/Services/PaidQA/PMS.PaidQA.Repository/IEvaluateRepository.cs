using PMS.PaidQA.Domain.Entities;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
namespace PMS.PaidQA.Repository
{
    public interface IEvaluateRepository : IRepository<Evaluate>
    {
        Task<int> Count(string str_Where, object param);

        /// <summary>
        /// 获取达人平均评分
        /// </summary>
        Task<double> GetAvgScoreByTalentUserID(Guid talentUserID);

        Task<(IEnumerable<Evaluate>, int)> PageByAnwserUserID(Guid anwserUserID, int pageIndex = 1, int pageSize = 10, Guid? tagID = null);
    }
}
