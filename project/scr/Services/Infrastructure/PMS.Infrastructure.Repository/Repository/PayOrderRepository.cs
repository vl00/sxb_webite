using PMS.Infrastructure.Domain.IRepositories;
using ProductManagement.Framework.MSSQLAccessor.DBContext;

namespace PMS.Infrastructure.Repository.Repository
{
    public class PayOrderRepository : IPayOrderRepository
    {
        private FinanceDBContext _dbContext;
        public PayOrderRepository(FinanceDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public int GetPayedOrderCount()
        {
            string sql = @"SELECT count(1) FROM [iSchoolFinance].[dbo].[PayOrder] WHERE [System] = N'0' AND [OrderStatus] <> N'1';";
            return _dbContext.QuerySingle<int>(sql, new { });
        }
    }
}
