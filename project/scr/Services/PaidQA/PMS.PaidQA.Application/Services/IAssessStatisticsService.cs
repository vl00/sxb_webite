using PMS.PaidQA.Domain.Entities;
using PMS.PaidQA.Domain.Enums;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace PMS.PaidQA.Application.Services
{
    public interface IAssessStatisticsService : IApplicationService<AssessStatisticsInfo>
    {
        Task<bool> Exist(AssessType type, DateTime date);
        Task<AssessStatisticsInfo> Get(AssessType assessType, DateTime date);
        Task<IEnumerable<AssessStatisticsInfo>> GetUnCountStatistics(int count = 0);
    }
}
