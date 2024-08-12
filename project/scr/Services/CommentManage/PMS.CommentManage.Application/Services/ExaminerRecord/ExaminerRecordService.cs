using PMS.CommentsManage.Application.IServices;
using PMS.CommentsManage.Domain.Entities.Total;
using PMS.CommentsManage.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.CommentsManage.Application.Services.ExaminerRecord
{
    public class ExaminerRecordService : IExaminerRecordService
    {
        public IExaminerRecordRepository _recordRepository;

        public ExaminerRecordService(IExaminerRecordRepository recordRepository) 
        {
            _recordRepository = recordRepository;
        }

        public int ExaminerTotal(Guid UserId, bool ExaminerStatus) 
        {
            return _recordRepository.ExaminerTotal(UserId, ExaminerStatus);
        }

        public ExaminerTotal GetCurrentExaminerCount(Guid AdminId) 
        {
            return _recordRepository.GetCurrentExaminerCount(AdminId);
        }
    }
}
