using PMS.PaidQA.Domain.Entities;
using ProductManagement.Framework.MSSQLAccessor.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMS.PaidQA.Repository
{
    public class AssessQuestionRepository : Repository<AssessQuestionInfo, PaidQADBContext>, IAssessQuestionRepository
    {
        PaidQADBContext _paidQADBContext;
        public AssessQuestionRepository(PaidQADBContext paidQADBContext) : base(paidQADBContext)
        {
            _paidQADBContext = paidQADBContext;
        }
    }
}
