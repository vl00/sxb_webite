using PMS.PaidQA.Domain.Entities;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace PMS.PaidQA.Repository
{
    public interface IGradeRepository : IRepository<Grade>
    {
        Task<int> Count(string str_Where, object param);

        Task<IEnumerable<Grade>> GetByTalentUserID(Guid talentUserID);

        Task<int> RemoveTalentGreades(Guid talentUserID);
        Task<int> AddTalentGreades(Guid talentUserID, IEnumerable<Guid> gradeIDs);
    }
}
