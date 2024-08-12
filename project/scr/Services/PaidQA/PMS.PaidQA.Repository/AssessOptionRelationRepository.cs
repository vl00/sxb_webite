using PMS.PaidQA.Domain.Entities;
using ProductManagement.Framework.MSSQLAccessor.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMS.PaidQA.Repository
{
    public class AssessOptionRelationRepository : Repository<AssessOptionRelationInfo, PaidQADBContext>, IAssessOptionRelationRepository
    {
        PaidQADBContext _paidQADBContext;
        public AssessOptionRelationRepository(PaidQADBContext paidQADBContext) : base(paidQADBContext)
        {
            _paidQADBContext = paidQADBContext;
        }

    }
}
