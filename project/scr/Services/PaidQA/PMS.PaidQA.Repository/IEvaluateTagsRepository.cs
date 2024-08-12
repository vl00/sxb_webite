using PMS.PaidQA.Domain.Entities;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using PMS.PaidQA.Domain.EntityExtend;

namespace PMS.PaidQA.Repository
{
    public interface IEvaluateTagsRepository : IRepository<EvaluateTags>
    {
        Task<IEnumerable<EvaluateTagCountingExtend>> GetEvaluateTagCountingByTalentUserID(Guid talentUserID);

        Task<IEnumerable<KeyValuePair<Guid, IEnumerable<EvaluateTags>>>> GetByEvaluateIDs(IEnumerable<Guid> ids);
    }
}
