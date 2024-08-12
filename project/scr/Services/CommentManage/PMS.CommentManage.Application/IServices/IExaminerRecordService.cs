using PMS.CommentsManage.Domain.Entities.Total;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.CommentsManage.Application.IServices
{
    public interface IExaminerRecordService
    {
        int ExaminerTotal(Guid UserId, bool ExaminerStatus);
        ExaminerTotal GetCurrentExaminerCount(Guid AdminId);
    }
}
