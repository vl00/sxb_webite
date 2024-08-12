using PMS.CommentsManage.Domain.Entities;
using PMS.CommentsManage.Domain.Entities.Total;
using ProductManagement.Infrastructure.AppService;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.CommentsManage.Domain.IRepositories
{
    /// <summary>
    /// 审核记录日志
    /// </summary>
    public interface IExaminerRecordRepository : IAppService<ExaminerRecord>
    {
        int ExaminerTotal(Guid UserId, bool ExaminerStatus);

        ExaminerTotal GetCurrentExaminerCount(Guid AdminId);
    }
}
