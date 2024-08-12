using PMS.PaidQA.Domain.Entities;
using PMS.PaidQA.Domain.EntityExtend;
using PMS.PaidQA.Repository;
using PMS.UserManage.Application.IServices;
using ProductManagement.Framework.Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMS.PaidQA.Application.Services
{

    public class RecommentTalentService : ApplicationService<RecommentTalent>, IRecommentTalentService
    {
        IRecommentTalentRepository _recommentTalentRepository;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="recommentTalentRepository"></param>
        public RecommentTalentService(IRecommentTalentRepository recommentTalentRepository) : base(recommentTalentRepository)
        {
            _recommentTalentRepository = recommentTalentRepository;
        }

        /// <summary>
        /// 专家列表，按用户关注专家领域查询，按粉丝排序
        /// </summary>
        /// <returns></returns>
        public async Task<List<TalentExtend>> GetTalentList(string userId)
        {
            return await _recommentTalentRepository.GetTalentList(userId);
        }

        /// <summary>
        /// 上学问专家列表
        /// </summary>
        /// <returns></returns>
        public async Task<List<TalentExtend>> GetTalentList()
        {
            return await _recommentTalentRepository.GetTalentList();
        }
    }
}
