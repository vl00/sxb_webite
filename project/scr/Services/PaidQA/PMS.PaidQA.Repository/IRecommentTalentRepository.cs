using PMS.PaidQA.Domain.Entities;
using PMS.PaidQA.Domain.EntityExtend;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PMS.PaidQA.Repository
{
    public interface IRecommentTalentRepository : IRepository<RecommentTalent>
    {
        /// <summary>
        /// 达人列表，按用户关注达人领域查询，按粉丝排序
        /// </summary>
        /// <returns></returns>
        Task<List<TalentExtend>> GetTalentList(string userId);

        /// <summary>
        /// 上学问达人列表
        /// </summary>
        /// <returns></returns>
        Task<List<TalentExtend>> GetTalentList();
    }
}
