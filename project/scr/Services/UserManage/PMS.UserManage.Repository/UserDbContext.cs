using System;
using Microsoft.Extensions.Logging;
using ProductManagement.Framework.MSSQLAccessor;

namespace PMS.UserManage.Repository
{
    public class UserDbContext : DBContext<UserDbContext>
    {
        public UserDbContext(ConnectionsManager<UserDbContext> connectionsManager, ILogger<DBContext<UserDbContext>> log):base(connectionsManager, log)
        {
        }
        public class DbNameHelper
        {
            public static string ISchoolTopicCircle { get; set; } = "[iSchoolTopicCircle].[dbo]";

        }
    }
}
