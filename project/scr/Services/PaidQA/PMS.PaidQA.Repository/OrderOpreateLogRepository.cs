using PMS.PaidQA.Domain.Entities;
using ProductManagement.Framework.MSSQLAccessor.DBContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.PaidQA.Repository
{
    public class OrderOpreateLogRepository:Repository<OrderOpreateLog, PaidQADBContext>, IOrderOpreateLogRepository
    {
        PaidQADBContext _paidQADBContext;
        public OrderOpreateLogRepository(PaidQADBContext dBContext) : base(dBContext)
        {
            _paidQADBContext = dBContext;
        }
    }
}
