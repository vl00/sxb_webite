using PMS.PaidQA.Domain.Entities;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace PMS.PaidQA.Application.Services
{
    public interface IAssessOptionRelationService : IApplicationService<AssessOptionRelationInfo>
    {
        Task<IEnumerable<AssessOptionRelationInfo>> GetByQID(Guid qid, Guid? firstOptionID = null, Guid? secondOptionID = null);
    }
}
