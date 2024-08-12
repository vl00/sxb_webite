using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PMS.CommentsManage.Application.IServices;
using PMS.CommentsManage.Application.ModelDto;
using PMS.CommentsManage.Domain.Common;
using PMS.CommentsManage.Domain.Entities;
using PMS.CommentsManage.Domain.IRepositories;
using ProductManagement.Framework.Foundation;
using ProductManagement.Infrastructure.Toolibrary;
using Sxb.Web.Models;
using Sxb.Web.Models.Common;
using Sxb.Web.Response;

namespace Sxb.Web.Controllers
{
    [Authorize]
    public class ReportController : BaseController
    {

        //点评回复
        private ISchoolCommentReplyService _replyService;
        //回答
        private IQuestionsAnswersInfoService _questionsAnswers;

        //点评举报
        private ICommentReportService _commentReportService;
        //问题举报
        private IAnswerReportRepository _answerReport;
        //回答举报

        //问答举报
        private IAnswerReportService _reportService;

        //举报原因
        private IReportTypeService _reportTypeService;

        private readonly ImageSetting _setting;
        private ISchoolImageService _imageService;


        public ReportController(IOptions<ImageSetting> set, IAnswerReportRepository answerReport, IQuestionsAnswersInfoService questionsAnswers, ISchoolCommentReplyService replyService, IReportTypeService reportTypeService,ICommentReportService commentReportService, IAnswerReportService reportService, ISchoolImageService imageService)
        {
            _commentReportService = commentReportService;
            _reportService = reportService;
            _reportTypeService = reportTypeService;
            _setting = set.Value;
            _imageService = imageService;
            _replyService = replyService;
            _questionsAnswers = questionsAnswers;
            _answerReport = answerReport;
        }
        public IActionResult Index(Guid dataSource,int reportType,Guid AnswerId = default(Guid),Guid AnswerReplyId = default(Guid))
        {
            ViewBag.Reports = _reportTypeService.GetReportTypes();

            Dictionary<string, string> keys = new Dictionary<string, string>
            {
                { "dataSource", dataSource.ToString() },
                { "reportType", reportType.ToString() },
                { "answer", AnswerId.ToString() },
                { "answerreplyid", AnswerReplyId.ToString() }
            };
            ViewBag.ReportDataSource = keys;

            return View();
        }

        /// <summary>
        /// 举报
        /// </summary>
        /// <param name="DataSource"></param>
        /// <param name="ReportReasonType"></param>
        /// <param name="ReportDetail"></param>
        /// <param name="ReportType"></param>
        /// <returns></returns>
        public ResponseResult Report()
        {
            try
            {
                Guid Id = Guid.Parse(HttpContext.Request.Form["DataSourceId"]);
                ReportTypeEnum ReportReasonType = (ReportTypeEnum)int.Parse(HttpContext.Request.Form["ReportReasonType"]);
                string ReportDetail = HttpContext.Request.Form["ReportDetail"];
                int ReportType = int.Parse(HttpContext.Request.Form["ReportType"]);
                Guid ReportId = Guid.Parse(HttpContext.Request.Form["Id"]);

                string image = HttpContext.Request.Form["imagers"].ToString();
                List<string> images = new List<string>();
                if (image != "")
                {
                    images.AddRange(image.Split(",").ToList());
                }


                Guid ImagerId = default(Guid);

                Guid userId = Guid.Empty;
                if (User.Identity.IsAuthenticated)
                {
                    var user = User.Identity.GetUserInfo();
                    userId = user.UserId;
                }

                //举报点评、及点评回复
                if (ReportReasonType == ReportTypeEnum.Comment || ReportReasonType == ReportTypeEnum.CommentReplay)
                {
                    CommentReportDto schoolCommentReport = new CommentReportDto
                    {
                        Id = ReportId,
                        ReportUserId = userId,
                        ReportDataType = ReportReasonType == ReportTypeEnum.Comment ? ReportDataType.DataSource : ReportDataType.Replay,
                        ReportDetail = ReportDetail,
                        ReportReasonType = ReportType
                    };

                    if (ReportReasonType == ReportTypeEnum.CommentReplay)
                    {
                        var commentReply = _replyService.GetCommentReply(Id);
                        schoolCommentReport.CommentId = commentReply.SchoolCommentId;
                        schoolCommentReport.ReplayId = commentReply.Id;
                    }
                    else
                    {
                        schoolCommentReport.CommentId = Id;
                    }
                    ImagerId = schoolCommentReport.Id;
                    _commentReportService.AddReport(schoolCommentReport);
                }
                //问题 / 问题回答/ 问题回复 举报
                else if (ReportReasonType == ReportTypeEnum.Question || ReportReasonType == ReportTypeEnum.Answer)
                {
                    QuestionsAnswersReport answersReport = new QuestionsAnswersReport
                    {
                        Id = ReportId,
                        ReportDataType = ReportReasonType == ReportTypeEnum.Question ? ReportDataType.DataSource : ReportDataType.Replay,
                        ReportUserId = userId,

                        ReportDetail = ReportDetail,
                        ReportTime = DateTime.Now,
                        ReportReasonType = ReportType
                    };

                    if (ReportReasonType == ReportTypeEnum.Question)
                    {
                        answersReport.QuestionId = Id;
                    }
                    else
                    {
                        var answerReply = _questionsAnswers.GetModelById(Id);
                        answersReport.QuestionId = answerReply.QuestionInfoId;

                        if(answerReply.ParentId == null)
                        {
                            answersReport.QuestionsAnswersInfoId = answerReply.Id;
                        }
                        else
                        {
                            answersReport.QuestionsAnswersInfoId = answerReply.ParentId;
                            answersReport.AnswersReplyId = answerReply.Id;
                        }

                    }
                    _answerReport.Add(answersReport);
                }

                List<SchoolImage> imgs = new List<SchoolImage>();
                //上传图片
                if (images.Count() > 0)
                {
                    //需要上传图片
                    imgs = new List<SchoolImage>();

                    for (int i = 0; i < images.Count(); i++)
                    {
                        SchoolImage commentImg = new SchoolImage
                        {
                            DataSourcetId = ImagerId,
                            ImageType = ImageType.Report,
                            ImageUrl = images[i]
                        };
                       
                        //图片上传成功，提交数据库
                        imgs.Add(commentImg);
                    }
                }

                if (imgs.Count != 0)
                {
                    foreach (var img in imgs)
                    {
                        _imageService.AddSchoolImage(img);
                    }
                }

                return ResponseResult.Success();
            }
            catch (Exception ex)
            {
                return ResponseResult.Failed(ex.Message);
            }
        }

    }
}