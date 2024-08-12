using Microsoft.Extensions.Logging;

namespace ProductManagement.Framework.MSSQLAccessor.DBContext
{
    public class FinanceDBContext : DBContext<FinanceDBContext>
    {
        public FinanceDBContext(ConnectionsManager<FinanceDBContext> connectionsManager, ILogger<DBContext<FinanceDBContext>> log) : base(connectionsManager, log)
        {
        }
    }
}
