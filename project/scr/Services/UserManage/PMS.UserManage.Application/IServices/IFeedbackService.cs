using Microsoft.AspNetCore.Http;
using PMS.UserManage.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.UserManage.Application.IServices
{
    public interface IFeedbackService
    {
        bool AddReport(Report reportInfo);
        bool AddFeedback(Feedback feedback);

        bool AddFeedbackImg(List<Feedback_Img> imgInfo);
        bool AddfeedbackCorrect(Feedback_corrected feedback);
        List<Report_Img> UploadReportImg(Guid report_id, IFormFileCollection images);
        List<Feedback_Img> UploadFeedbackImg(Guid feedback_id, IFormFileCollection images);
    }
}
