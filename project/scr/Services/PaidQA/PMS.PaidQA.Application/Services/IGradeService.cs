using PMS.PaidQA.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PMS.PaidQA.Application.Services
{
    public interface IGradeService : IApplicationService<Grade>
    {
        Task<IEnumerable<Grade>> GetByTalentUserID(Guid talentUserID);
        Task<Dictionary<Guid, IEnumerable<Grade>>> GetByTalentUserIDs(IEnumerable<Guid> talentUserIDs);
        bool RemoveTalentGrades(Guid talentUserID);
        bool AddTalentGrades(Guid talentUserID, IEnumerable<Guid> gradeIDs);

        Task<IEnumerable<Grade>> GetAllGrades();
    }
}
