using PMS.PaidQA.Domain.Entities;
using ProductManagement.Framework.MSSQLAccessor.DBContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.PaidQA.Repository
{
   public  class EvaluateTagRelationRepository : Repository<EvaluateTagRelation, PaidQADBContext>, IEvaluateTagRelationRepository
    {
        PaidQADBContext _paidQADBContext;
        public EvaluateTagRelationRepository(PaidQADBContext dBContext) : base(dBContext)
        {
            _paidQADBContext = dBContext;
        }

    }
}
