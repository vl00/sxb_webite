using System;
using Microsoft.Extensions.Logging;
using ProductManagement.Framework.MSSQLAccessor;

namespace ImportAndExport.Repository
{
    public class DataDbContext : DBContext<DataDbContext>
    {
        public DataDbContext(ConnectionsManager<DataDbContext> connectionsManager, ILogger<DBContext<DataDbContext>> log) : base(connectionsManager, log)
        {
        }
    }
}
