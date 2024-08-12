using PMS.PaidQA.Domain.Entities;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using PMS.PaidQA.Domain.EntityExtend;

namespace PMS.PaidQA.Application.Services
{
    public interface IEvaluateService : IApplicationService<Evaluate>
    {
        Task<IEnumerable<Evaluate>> GetByOrderIDs(IEnumerable<Guid> IDs);

        /// <summary>
        /// 获取专家平均频分
        /// </summary>
        Task<double> GetTalentAvgScope(Guid talentUserID);

        Task<bool> UserCreateEvaluate(Order order, Evaluate evaluate);

        /// <summary>
        /// 根据专家UserID获取便签统计数据
        /// </summary>
        /// <param name="talentUserID"></param>
        /// <returns></returns>
        Task<IEnumerable<EvaluateTagCountingExtend>> GetEvaluateTagCountingByTalentUserID(Guid talentUserID);

        /// <summary>
        /// 根据专家UserID分页获取评论
        /// </summary>
        /// <param name="tagID">评论标签ID</param>
        /// <returns></returns>
        Task<(IEnumerable<Evaluate>, int)> PageByTalentUserID(Guid talentUserID, int pageIndex = 1, int pageSize = 10, Guid? tagID = null);

        /// <summary>
        /// 获取所有的评价标签
        /// </summary>
        /// <returns></returns>
        IEnumerable<EvaluateTags> GetEvaluateTags();

        /// <summary>
        /// 评价超时，系统自动好评
        /// </summary>
        /// <returns></returns>
        Task<bool> AutoNiceEvaluation();
    }
}
