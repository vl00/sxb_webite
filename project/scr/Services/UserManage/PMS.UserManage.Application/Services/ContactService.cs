using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using PMS.Infrastructure.Domain.Entities;
using PMS.Infrastructure.Domain.IRepositories;
using PMS.UserManage.Application.ModelDto;
using PMS.UserManage.Domain.Common;
using ProductManagement.Framework.Foundation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PMS.UserManage.Application.Services
{
    public class ContactService
    {
        public IsHost _IsHost;
        private IFeedbackRepository _feedback;
        public ContactService(IFeedbackRepository feedback, IOptions<IsHost> IsHost) 
        {
            _feedback = feedback;
            _IsHost = IsHost.Value;
        }


        public bool AddFeedback(Guid userID, byte type, string text, IFormFileCollection images)
        {
            Feedback feedbackInfo = new Feedback()
            {
                Id = Guid.NewGuid(),
                UserID = userID,
                type = type,
                text = text,
                time = DateTime.Now
            };
            if (_feedback.AddFeedback(feedbackInfo))
            {
                if (images.Count > 0)
                {
                    AddFeedbackImg(feedbackInfo.Id, images);
                }
                return true;
            }
            return false;
        }
        private bool AddFeedbackImg(Guid feedback_id, IFormFileCollection images)
        {
            Stream fileStream;
            string ext;
            List<Feedback_Img> feedback_imgs = new List<Feedback_Img>();
            foreach (var image in images)
            {
                Guid fileID = Guid.NewGuid();
                fileStream = image.OpenReadStream();
                ext = Path.GetExtension(images[0].FileName);
                FileUploadResponseModel result = new FileUploadResponseModel();
                result = HttpHelper.HttpPost<FileUploadResponseModel>($"{_IsHost.FileHost}/Upload/Feedback?filename=" + feedback_id + "/" + fileID + ext
                    , fileStream);
                if (result.status == 0)
                {
                    feedback_imgs.Add(new Feedback_Img() { Feedback_Id = feedback_id, Id = fileID, url = result.url });
                }
            }
            if (feedback_imgs.Count > 0)
            {
                return _feedback.AddFeedbackImg(feedback_imgs);
            }
            return false;
        }
        public bool AddReport(Guid userID, Guid dataID, byte dataType, string reason, string description, string evidenceURL, IFormFileCollection images)
        {
            Report reportInfo = new Report()
            {
                Id = Guid.NewGuid(),
                UserID = userID,
                DataID = dataID,
                DataType = dataType,
                Reason = reason,
                Description = description,
                EvidenceURL = evidenceURL,
                Time = DateTime.Now
            };
            if (_feedback.AddReport(reportInfo))
            {
                if (images.Count > 0)
                {
                    AddReportImg(reportInfo.Id, images);
                }
                return true;
            }
            return false;
        }
        private bool AddReportImg(Guid report_id, IFormFileCollection images)
        {
            Stream fileStream;
            string ext;
            List<Report_Img> report_imgs = new List<Report_Img>();
            foreach (var image in images)
            {
                Guid fileID = Guid.NewGuid();
                fileStream = image.OpenReadStream();
                ext = Path.GetExtension(images[0].FileName);
                FileUploadResponseModel result = new FileUploadResponseModel();
                result = HttpHelper.HttpPost<FileUploadResponseModel>($"{_IsHost.FileHost}/Upload/Report?filename=" + report_id + "/" + fileID + ext
                    , fileStream);
                if (result != null && result.status == 0)
                {
                    report_imgs.Add(new Report_Img() { Report_Id = report_id, Id = fileID, url = result.url });
                }
            }
            if (report_imgs.Count > 0)
            {
                return _feedback.AddReportImg(report_imgs);
            }
            return false;
        }

    }
}
