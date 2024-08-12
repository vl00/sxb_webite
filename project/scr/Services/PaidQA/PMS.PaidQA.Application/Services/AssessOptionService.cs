using PMS.PaidQA.Domain.Entities;
using PMS.PaidQA.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMS.PaidQA.Application.Services
{
    public class AssessOptionService : ApplicationService<AssessOptionInfo>, IAssessOptionService
    {
        IAssessOptionRepository _assessOptionRepository;
        public AssessOptionService(IAssessOptionRepository assessOptionRepository) : base(assessOptionRepository)
        {
            _assessOptionRepository = assessOptionRepository;
        }

        public async Task<IEnumerable<AssessOptionInfo>> GetByIDs(IEnumerable<Guid> ids)
        {
            if (ids == null || !ids.Any()) return null;
            var str_Where = "ID in @ids";
            return await _assessOptionRepository.GetByAsync(str_Where, new { ids = ids.Distinct() });
        }

        public async Task<IEnumerable<AssessOptionInfo>> GetByShortIDs(IEnumerable<int> shortIDs)
        {
            if (shortIDs == null || !shortIDs.Any()) return null;
            return await _assessOptionRepository.GetByAsync("ShortID in @shortIDs", new { shortIDs });
        }
    }
}
