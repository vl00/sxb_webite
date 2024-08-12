using PMS.PaidQA.Domain.Entities;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace PMS.PaidQA.Application.Services
{
    public interface IAssessOptionService : IApplicationService<AssessOptionInfo>
    {
        Task<IEnumerable<AssessOptionInfo>> GetByShortIDs(IEnumerable<int> shortIDs);
        Task<IEnumerable<AssessOptionInfo>> GetByIDs(IEnumerable<Guid> ids);
    }
}
