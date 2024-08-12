using PMS.Infrastructure.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.Infrastructure.Domain.IRepositories
{
    public interface IFeedbackRepository
    {
        bool AddFeedbackImg(List<Feedback_Img> imgInfo);
        bool AddReport(Report reportInfo);

        bool AddFeedback(Feedback feedback);

        bool AddReportImg(List<Report_Img> imgInfo);
    }
}
