using PMS.PaidQA.Domain.Entities;
using PMS.PaidQA.Repository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PMS.PaidQA.Application.Services
{
    public class AssessOptionRelationService : ApplicationService<AssessOptionRelationInfo>, IAssessOptionRelationService
    {
        IAssessOptionRelationRepository _assessOptionRelationRepository;
        public AssessOptionRelationService(IAssessOptionRelationRepository assessOptionRelationRepository) : base(assessOptionRelationRepository)
        {
            _assessOptionRelationRepository = assessOptionRelationRepository;
        }

        public async Task<IEnumerable<AssessOptionRelationInfo>> GetByQID(Guid qid, Guid? firstOptionID = null, Guid? secondOptionID = null)
        {
            if (qid == Guid.Empty) return null;
            var str_Where = "QuestionID = @qid";
            if (firstOptionID.HasValue && firstOptionID.Value != Guid.Empty)
            {
                str_Where += " AND FirstOptionID = @firstOptionID";
            }
            if (secondOptionID.HasValue && secondOptionID.Value != Guid.Empty)
            {
                str_Where += " AND secondOptionID = @secondOptionID";
            }
            return await _assessOptionRelationRepository.GetByAsync(str_Where, new { qid, firstOptionID, secondOptionID });
        }
    }
}
