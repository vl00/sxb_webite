using PMS.PaidQA.Domain.Entities;
using PMS.PaidQA.Domain.EntityExtend;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PMS.PaidQA.Application.Services
{
    public interface IRecommentTalentService : IApplicationService<RecommentTalent>
    {
        /// <summary>
        /// 专家列表，按用户关注专家领域查询，按粉丝排序
        /// </summary>
        /// <returns></returns>
        Task<List<TalentExtend>> GetTalentList(string userId);

        /// <summary>
        /// 上学问专家列表
        /// </summary>
        /// <returns></returns>
        Task<List<TalentExtend>> GetTalentList();
    }
}