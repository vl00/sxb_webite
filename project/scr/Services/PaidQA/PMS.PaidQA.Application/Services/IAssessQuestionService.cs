using PMS.PaidQA.Domain.Entities;
using System.Threading.Tasks;
using System.Collections.Generic;
using PMS.PaidQA.Domain.Enums;

namespace PMS.PaidQA.Application.Services
{
    public interface IAssessQuestionService : IApplicationService<AssessQuestionInfo>
    {
        Task<IEnumerable<AssessQuestionInfo>> GetByType(AssessType type);
    }
}
