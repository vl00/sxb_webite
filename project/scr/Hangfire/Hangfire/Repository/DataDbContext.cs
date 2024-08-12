using Microsoft.Extensions.Logging;
using ProductManagement.Framework.MSSQLAccessor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hangfire.ConsoleWeb.Repository
{
    public class DataDbContext : DBContext<DataDbContext>
    {
        public DataDbContext(ConnectionsManager<DataDbContext> connectionsManager, ILogger<DBContext<DataDbContext>> log) : base(connectionsManager, log)
        {
        }
    }
}
