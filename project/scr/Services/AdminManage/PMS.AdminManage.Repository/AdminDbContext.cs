using System;
using Microsoft.Extensions.Logging;
using ProductManagement.Framework.MSSQLAccessor;

namespace PMS.AdminManage.Repository
{
    public class AdminDbContext : DBContext<AdminDbContext>
    {
        public AdminDbContext(ConnectionsManager<AdminDbContext> connectionsManager, ILogger<DBContext<AdminDbContext>> log):base(connectionsManager, log)
        {
        }
    }
}
