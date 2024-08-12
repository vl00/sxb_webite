using PMS.PaidQA.Domain.Entities;
using ProductManagement.Framework.MSSQLAccessor.DBContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.PaidQA.Repository
{
    public class SxbDarenEmailRepository : Repository<SxbDarenEmail, PaidQADBContext>, ISxbDarenEmailRepository
    {
        PaidQADBContext _paidQADBContext;

        public SxbDarenEmailRepository(PaidQADBContext dBContext) : base(dBContext)
        {
            _paidQADBContext = dBContext;
        }
    }
}
