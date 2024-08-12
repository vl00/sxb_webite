using System;
using System.Collections.Generic;
using PMS.CommentsManage.Domain.Common;
using PMS.CommentsManage.Domain.Entities;
using ProductManagement.Infrastructure.AppService;

namespace PMS.CommentsManage.Domain.IRepositories
{
    public interface ICommentReportRepository : IAppService<SchoolCommentReport>
    {
        List<SchoolCommentReport> PageCommentReport(int pageIndex, int pageSize, DateTime startTime, DateTime endTime, out int total, List<Guid> schoolIds = null);
    }
}
