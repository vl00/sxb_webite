using Microsoft.Extensions.Logging;

namespace ProductManagement.Framework.MSSQLAccessor.DBContext
{
    public class LiveDBContext : DBContext<LiveDBContext>
    {
        public LiveDBContext(ConnectionsManager<LiveDBContext> connectionsManager, ILogger<DBContext<LiveDBContext>> log) : base(connectionsManager, log)
        {

        }
    }
}
