using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PMS.UserManage.Application.IServices;
using PMS.UserManage.Application.ModelDto.ModelVo;
using PMS.UserManage.Domain.Common;
using PMS.UserManage.Domain.Entities;

namespace Sxb.UserCenter.Controllers
{
    public class ContactController : Base
    {
        public IsHost IsHost { get; }
        private IFeedbackService _feedback;
        private IApiService _apiService; 
        public ContactController(
            IFeedbackService feedback,
            IOptions<IsHost> _isHost,
            IApiService apiService)
        {
            _apiService = apiService;
            IsHost = _isHost.Value;
            _feedback = feedback;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Report(EnumSet.ReportType? type, Guid id)
        {
            if (type == null || id==Guid.Empty)
            {
                return Redirect("/");
            }
            Data data = null;
            if (type == EnumSet.ReportType.Article)
            {
                var vo = _apiService.GetArticlesByIds(new List<Guid>() { id });
                if (vo.data.Count == 0)
                {
                    return Redirect("/");
                }
                data = vo.data[0];
            }
            ViewBag.Type = type;
            ViewBag.DataID = id;
            return View(data);
        }
        public IActionResult ReportSubmit(IFormFileCollection images, string reason, string description, string evidenceURL, Guid id, byte type)
        {
            PMS.UserManage.Domain.Common.RootModel ret = new PMS.UserManage.Domain.Common.RootModel();
            /*
                userID, id, type, reason, description, evidenceURL, images
             */
             var report = new PMS.UserManage.Domain.Entities.Report()
             {
                 Id = Guid.NewGuid(),
                 UserID = userID,
                 DataID = id,
                 DataType = type,
                 Reason = reason,
                 Description = description,
                 EvidenceURL = evidenceURL
            };

            if (images.Any()) 
            {
                report.Report_Imgs = _feedback.UploadReportImg(report.Id, images);
            }

            if (!_feedback.AddReport(report))
            {
                ret.status = 1;
                ret.errorDescription = "提交失败";
            }   
            return Json(ret);
        }
        public IActionResult Feedback()
        {
            return View();
        }
        public IActionResult FeedbackSubmit(IFormFileCollection images, string text, byte type)
        {
            PMS.UserManage.Domain.Common.RootModel ret = new PMS.UserManage.Domain.Common.RootModel();

            Feedback feedback = new Feedback()
            {
                UserID = userID,
                type = type,
                text = text
            };

            if (!_feedback.AddFeedback(feedback))
            {
                ret.status = 1;
                ret.errorDescription = "提交失败";
            }
            else 
            {
                if (images.Any()) 
                {
                    var feedbackImg = _feedback.UploadFeedbackImg(feedback.Id, images);
                    _feedback.AddFeedbackImg(feedbackImg);
                }
            }
            return Json(ret);
        }
        public IActionResult QA()
        {
            return View();
        }
        public IActionResult Form()
        {
            return RedirectToAction("Feedback", new { type = 100 });
        }
    }
}