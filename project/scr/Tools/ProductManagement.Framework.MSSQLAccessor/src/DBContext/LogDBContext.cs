using Microsoft.Extensions.Logging;

namespace ProductManagement.Framework.MSSQLAccessor.DBContext
{
    public class LogDBContext : DBContext<LogDBContext>
    {
        public LogDBContext(ConnectionsManager<LogDBContext> connectionsManager, ILogger<DBContext<LogDBContext>> log) : base(connectionsManager, log)
        {
        }
    }
}
