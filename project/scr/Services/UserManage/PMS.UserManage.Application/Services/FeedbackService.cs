using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using PMS.UserManage.Application.IServices;
using PMS.UserManage.Domain.Common;
using PMS.UserManage.Domain.Entities;
using PMS.UserManage.Domain.IRepositories;
using ProductManagement.Framework.Foundation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PMS.UserManage.Application.Services
{
    public class FeedbackService : IFeedbackService
    {
        private IsHost IsHost { get; }
        private IFeedbackRepository _repository;
        public FeedbackService(IFeedbackRepository repository,IOptions<IsHost> _isHost) 
        {
            _repository = repository;
            IsHost = _isHost.Value;
        }

        public bool AddFeedback(Feedback feedback)
        {
            return _repository.AddFeedback(feedback);
        }

        public bool AddfeedbackCorrect(Feedback_corrected feedback)
        {
            return _repository.AddfeedbackCorrect(feedback);
        }

        public bool AddFeedbackImg(List<Feedback_Img> imgInfo)
        {
            return _repository.AddFeedbackImg(imgInfo);
        }

        public bool AddReport(Report reportInfo)
        {
            return _repository.AddReport(reportInfo);
        }


        public List<Report_Img> UploadReportImg(Guid report_id, IFormFileCollection images)
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
                result = HttpHelper.HttpPost<FileUploadResponseModel>($"{IsHost.FileHost}/Upload/Report?filename=" + report_id + "/" + fileID + ext
                    , fileStream);
                if (result != null && result.status == 0)
                {
                    report_imgs.Add(new Report_Img() { Report_Id = report_id, Id = fileID, url = result.url });
                }
            }
            return report_imgs;
        }

        public List<Feedback_Img> UploadFeedbackImg(Guid feedback_id, IFormFileCollection images)
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
                result = HttpHelper.HttpPost<FileUploadResponseModel>($"{IsHost.FileHost}/Upload/Feedback?filename=" + feedback_id + "/" + fileID + ext
                    , fileStream);
                if (result.status == 0)
                {
                    feedback_imgs.Add(new Feedback_Img() { Feedback_Id = feedback_id, Id = fileID, url = result.url });
                }
            }
            return feedback_imgs;
        }

    }
}
