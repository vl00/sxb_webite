using PMS.Infrastructure.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.Infrastructure.Application.IService
{
    public interface IReportService
    {
        bool AddReport(Report reportInfo);

        bool AddFeedback(Feedback feedback);

        bool AddFeedbackImg(List<Feedback_Img> imgInfo);
    }
}
