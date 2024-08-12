using PMS.PaidQA.Domain.Entities;
using ProductManagement.Framework.MSSQLAccessor.DBContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.PaidQA.Repository
{
    public class PayWalletLogRepository : Repository<PayWalletLog, PaidQADBContext>, IPayWalletLogRepository
    {
        private PaidQADBContext _paidQADBContext;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dBContext"></param>
        public PayWalletLogRepository(PaidQADBContext dBContext) : base(dBContext)
        {
            _paidQADBContext = dBContext;
        }
    }
}
