using Microsoft.Extensions.Logging;
using ProductManagement.Framework.MSSQLAccessor;

namespace ProductManagement.Framework.MSSQLAccessor.DBContext
{
    public class TopicCircleDBContext : DBContext<TopicCircleDBContext>
    {
        public TopicCircleDBContext(ConnectionsManager<TopicCircleDBContext> connectionsManager, ILogger<DBContext<TopicCircleDBContext>> log) : base(connectionsManager, log)
        {
        }
    }

    public class DbNameHelper
    {
        public static string ISchoolUser { get; set; } = "[iSchoolUser].[dbo]";

    }
}
