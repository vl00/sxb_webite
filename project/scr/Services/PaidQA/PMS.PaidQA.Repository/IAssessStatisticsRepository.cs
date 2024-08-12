using PMS.PaidQA.Domain.Entities;
using PMS.PaidQA.Domain.Enums;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace PMS.PaidQA.Repository
{
    public interface IAssessStatisticsRepository : IRepository<AssessStatisticsInfo>
    {
        Task<bool> CheckExist(AssessType type, DateTime date);
        Task<AssessStatisticsInfo> Get(AssessType assessType, DateTime date);
        Task<IEnumerable<AssessStatisticsInfo>> GetUnCounteds(int count = 0);
    }
}
