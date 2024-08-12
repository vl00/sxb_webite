using Microsoft.Extensions.Logging;
using ProductManagement.Framework.MSSQLAccessor;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.Infrastructure.Repository
{
    public class CityDbContext : DBContext<CityDbContext>
    {
        public CityDbContext(ConnectionsManager<CityDbContext> connectionsManager, ILogger<DBContext<CityDbContext>> log) : base(connectionsManager, log)
        {
        }
    }
}
