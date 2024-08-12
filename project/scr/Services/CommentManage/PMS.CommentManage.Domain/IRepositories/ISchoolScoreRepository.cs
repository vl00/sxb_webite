using PMS.CommentsManage.Domain.Entities;
using ProductManagement.Infrastructure.AppService;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace PMS.CommentsManage.Domain.IRepositories
{
    public interface ISchoolScoreRepository : IAppService<SchoolScore>
    {
        int ExecuteNonQuery(string sql, params SqlParameter[] paras);

        IEnumerable<SchoolScore> Query(string sql, params SqlParameter[] paras);
    }
}
