using PMS.PaidQA.Domain.Entities;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using PMS.PaidQA.Domain.Enums;

namespace PMS.PaidQA.Application.Services
{
    public interface IAssessService : IApplicationService<AssessInfo>
    {
        Task<IEnumerable<AssessInfo>> GetByUserID(Guid userID, AssessStatus status = AssessStatus.Unknow);
        Task<IEnumerable<AssessInfo>> GetByCountingTime(DateTime countingTime);
    }
}
