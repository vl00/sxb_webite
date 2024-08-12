using PMS.PaidQA.Domain.Entities;
using PMS.PaidQA.Domain.Enums;
using PMS.PaidQA.Repository;
using ProductManagement.Framework.Cache.Redis;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PMS.PaidQA.Application.Services
{
    public class AssessQuestionService : ApplicationService<AssessQuestionInfo>, IAssessQuestionService
    {
        IAssessQuestionRepository _assessQuestionRepository;
        IEasyRedisClient _easyRedisClient;
        public AssessQuestionService(IAssessQuestionRepository assessQuestionRepository, IEasyRedisClient easyRedisClient) : base(assessQuestionRepository)
        {
            _easyRedisClient = easyRedisClient;
            _assessQuestionRepository = assessQuestionRepository;
        }

        public async Task<IEnumerable<AssessQuestionInfo>> GetByType(AssessType type)
        {
            if (type == AssessType.Unknow) return null;

            var finds = await _easyRedisClient.GetOrAddAsync($"Assess:Question_{(int)type}", async () =>
            {
                return await _assessQuestionRepository.GetByAsync("AssessType = @type", new { type = (int)type }, "Level");
            }, TimeSpan.FromHours(12));
            return finds;
        }
    }
}
