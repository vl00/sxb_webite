using PMS.PaidQA.Domain.Entities;
using PMS.PaidQA.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PMS.PaidQA.Domain.EntityExtend;

namespace PMS.PaidQA.Application.Services
{
    public class EvaluateTagsService : ApplicationService<EvaluateTags>, IEvaluateTagsService
    {
        IEvaluateTagsRepository _evaluateTagsRepository;
        public EvaluateTagsService(IEvaluateTagsRepository evaluateTagsRepository) : base(evaluateTagsRepository)
        {
            _evaluateTagsRepository = evaluateTagsRepository;
        }

        public async Task<IEnumerable<KeyValuePair<Guid, IEnumerable<EvaluateTags>>>> GetByEvaluateIDs(IEnumerable<Guid> ids)
        {
            if (ids == null || !ids.Any()) return null;
            return await _evaluateTagsRepository.GetByEvaluateIDs(ids);
        }
    }
}
