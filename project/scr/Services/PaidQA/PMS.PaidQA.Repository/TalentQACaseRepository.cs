using PMS.PaidQA.Domain.Entities;
using ProductManagement.Framework.MSSQLAccessor.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMS.PaidQA.Repository
{
    public class TalentQACaseRepository : Repository<TalentQACase, PaidQADBContext>, ITalentQACaseRepository
    {
        PaidQADBContext _paidQADBContext;
        public TalentQACaseRepository(PaidQADBContext paidQADBContext) : base(paidQADBContext)
        {
            _paidQADBContext = paidQADBContext;
        }

    }
}
