using Microsoft.Extensions.Logging;

namespace ProductManagement.Framework.MSSQLAccessor.DBContext
{
    public class PaidQADBContext : DBContext<PaidQADBContext>
    {
        public PaidQADBContext(ConnectionsManager<PaidQADBContext> connectionsManager, ILogger<DBContext<PaidQADBContext>> log) : base(connectionsManager, log)
        {
        }
    }
}
