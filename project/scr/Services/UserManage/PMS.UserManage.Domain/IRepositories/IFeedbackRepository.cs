using PMS.UserManage.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.UserManage.Domain.IRepositories
{
    public interface IFeedbackRepository
    {
        bool AddFeedback(Feedback feedback);

        bool AddFeedbackImg(List<Feedback_Img> imgInfo);
        bool AddfeedbackCorrect(Feedback_corrected feedback);

        bool AddReport(Report reportInfo);

        bool AddReportImg(List<Report_Img> imgInfo);
    }
}
