using Microsoft.EntityFrameworkCore.Internal;
using PMS.TopicCircle.Domain.Entities;
using PMS.TopicCircle.Domain.Repositories;
using ProductManagement.Framework.MSSQLAccessor;
using ProductManagement.Framework.MSSQLAccessor.DBContext;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace PMS.TopicCircle.Repository
{
    public class CircleAccessLogRepository : Repository<CircleAccessLog, TopicCircleDBContext>, ICircleAccessLogRepository
    {
        TopicCircleDBContext _db;
        public CircleAccessLogRepository(TopicCircleDBContext dBContext) : base(dBContext)
        {
            _db = dBContext;
        }
        public async override Task<bool> UpdateAsync(CircleAccessLog entity, IDbTransaction transaction = null, string[] fields = null)
        {
            List<string> setvalues = new List<string>();
            foreach (var field in fields)
            {
                setvalues.Add($"[{field}] = @{field}");
            }
            string sql = $" UPDATE [CIRCLEACCESSLOG] SET {string.Join(",", setvalues)} WHERE  CIRCLEID=@CircleId AND USERID=@UserId ";
            return await _db.ExecuteAsync(sql, entity, transaction) > 0;
        }
    }
}
