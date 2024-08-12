using PMS.PaidQA.Domain.Entities;
using PMS.PaidQA.Domain.Enums;
using PMS.PaidQA.Repository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PMS.PaidQA.Application.Services
{
    public class AssessStatisticsService : ApplicationService<AssessStatisticsInfo>, IAssessStatisticsService
    {
        IAssessStatisticsRepository _assessStatisticsRepository;
        public AssessStatisticsService(IAssessStatisticsRepository assessStatisticsRepository) : base(assessStatisticsRepository)
        {
            _assessStatisticsRepository = assessStatisticsRepository;
        }

        public async Task<bool> Exist(AssessType type, DateTime date)
        {
            return await _assessStatisticsRepository.CheckExist(type, date);
        }

        public async Task<AssessStatisticsInfo> Get(AssessType assessType, DateTime date)
        {
            return await _assessStatisticsRepository.Get(assessType, date);
        }

        public async Task<IEnumerable<AssessStatisticsInfo>> GetUnCountStatistics(int count = 0)
        {
            return await _assessStatisticsRepository.GetUnCounteds(count);
        }
    }
}
