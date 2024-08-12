using PMS.Infrastructure.Application.IService;
using PMS.Infrastructure.Domain.Entities;
using PMS.Infrastructure.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.Infrastructure.Application.Service
{
    public class ReportService : IReportService
    {
        IFeedbackRepository _repository;
        public ReportService(IFeedbackRepository repository) 
        {
            _repository = repository;
        }

        public bool AddFeedback(Feedback feedback)
        {
            return _repository.AddFeedback(feedback);
        }

        public bool AddFeedbackImg(List<Feedback_Img> imgInfo)
        {
            return _repository.AddFeedbackImg(imgInfo);
        }

        public bool AddReport(Report reportInfo)
        {
            return _repository.AddReport(reportInfo);
        }
    }
}
