using Microsoft.Extensions.Logging;
using ProductManagement.Framework.MSSQLAccessor;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.Infrastructure.Repository
{
    public class JcDbContext : DBContext<JcDbContext>
    {
        public JcDbContext(ConnectionsManager<JcDbContext> connectionsManager, ILogger<DBContext<JcDbContext>> log) : base(connectionsManager, log)
        {
        }
    }
}
