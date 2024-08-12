using PMS.PaidQA.Domain.Entities;
using PMS.PaidQA.Domain.Enums;
using PMS.PaidQA.Repository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PMS.PaidQA.Application.Services
{
    public class AssessService : ApplicationService<AssessInfo>, IAssessService
    {
        IAssessRepository _assessRepository;
        public AssessService(IAssessRepository assessRepository) : base(assessRepository)
        {
            _assessRepository = assessRepository;
        }

        public async Task<IEnumerable<AssessInfo>> GetByCountingTime(DateTime countingTime)
        {
            var str_Where = $"CreateTime >= '{countingTime.ToString("yyyy-MM-dd")}' AND CreateTime < '{countingTime.AddDays(1).ToString("yyyy-MM-dd")}'";
            return await _assessRepository.GetByAsync(str_Where, new { }, "CreateTime Desc");
        }

        public async Task<IEnumerable<AssessInfo>> GetByUserID(Guid userID, AssessStatus status = AssessStatus.Unknow)
        {
            if (userID == Guid.Empty) return null;
            var str_Where = "UserID = @userID";
            if (status != AssessStatus.Unknow)
            {
                str_Where += " AND Status = @status";
            }
            return await _assessRepository.GetByAsync(str_Where, new { userID, status = (int)status }, "CreateTime Desc");
        }

    }
}
