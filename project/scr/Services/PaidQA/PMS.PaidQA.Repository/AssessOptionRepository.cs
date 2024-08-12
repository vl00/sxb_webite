using PMS.PaidQA.Domain.Entities;
using ProductManagement.Framework.MSSQLAccessor.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMS.PaidQA.Repository
{
    public class AssessOptionRepository : Repository<AssessOptionInfo, PaidQADBContext>, IAssessOptionRepository
    {
        PaidQADBContext _paidQADBContext;
        public AssessOptionRepository(PaidQADBContext paidQADBContext) : base(paidQADBContext)
        {
            _paidQADBContext = paidQADBContext;
        }

    }
}
