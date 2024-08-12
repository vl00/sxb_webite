using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using PMS.CommentsManage.Application.IServices;
using PMS.CommentsManage.Application.IServices.IMQService;
using PMS.CommentsManage.Domain.Common;
using PMS.CommentsManage.Domain.Entities;
using PMS.Infrastructure.Application.IService;
using PMS.School.Application.IServices;
using PMS.Search.Application.IServices;
using PMS.UserManage.Application.IServices;
using ProductManagement.API.Http.Interface;
using ProductManagement.Framework.Cache.Redis;
using ProductManagement.Framework.Foundation;
using Sxb.PCWeb.Common;
using Sxb.PCWeb.Models;
using Sxb.PCWeb.Response;
using Sxb.PCWeb.Utils;
using Sxb.PCWeb.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.DrawingCore;
using System.Linq;
using MediatR;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Sxb.PCWeb.Controllers
{
    using Microsoft.Extensions.Configuration;
    using PMS.MediatR.Events.Comment;
    using PMS.OperationPlateform.Application.IServices;
    using PMS.UserManage.Domain.Entities;
    using System.ComponentModel;
    using System.Web;

    public class CommonController : Controller
    {
        private readonly ImageSetting _setting;
        private readonly ICityInfoService _cityService;
        private readonly IEasyRedisClient _easyRedisClient;
        private readonly ISchoolService _schoolService;
        private readonly IUserServiceClient _userMessage;
        private readonly IUserService _userService;

        private readonly ISchoolCommentReplyService _schoolCommentReplyService;
        private readonly IQuestionsAnswersInfoService _questionsAnswersInfoService;

        private readonly ISchoolCommentService _commentService;
        private readonly IQuestionInfoService _questionInfo;

        private readonly ISearchService _searchService;

        private readonly IGiveLikeService _giveLikeService;

        private readonly ISchoolCommentLikeMQService _likeMQService;

        private readonly IAdvertisingOptionService _advertisingOptionService;

        private readonly IAdvertisingBaseService _advertisingBaseService;

        private readonly IFeedbackService _reportService;

        //事件总线
        private readonly ISchoolCommentLikeMQService _commentLikeMQService;

        IMediator _mediator;

        public CommonController(ICityInfoService cityService,
            IEasyRedisClient easyRedisClient,
            ISchoolService schoolService,
            IUserServiceClient userMessage,
            ISchoolCommentService commentService,
            IQuestionsAnswersInfoService questionsAnswersInfoService,
            IQuestionInfoService questionInfo,
            IUserService userService,
            ISearchService searchService,
            ISchoolCommentReplyService schoolCommentReplyService,
            IGiveLikeService giveLikeService,
            ISchoolCommentLikeMQService likeMQService,
            IAdvertisingOptionService advertisingOptionService,
            IAdvertisingBaseService advertisingBaseService,
            ISchoolCommentLikeMQService commentLikeMQService,
            IFeedbackService reportService,
            IOptions<ImageSetting> set,
            IMediator mediator)
        {
            _mediator = mediator;
            _cityService = cityService;
            _easyRedisClient = easyRedisClient;
            _setting = set.Value;
            _userService = userService;
            _giveLikeService = giveLikeService;
            _schoolCommentReplyService = schoolCommentReplyService;
            _userMessage = userMessage;

            _reportService = reportService;

            _questionsAnswersInfoService = questionsAnswersInfoService;
            _commentService = commentService;
            _questionInfo = questionInfo;

            _schoolService = schoolService;

            _likeMQService = likeMQService;

            _searchService = searchService;

            _advertisingOptionService = advertisingOptionService;

            _advertisingBaseService = advertisingBaseService;

            _commentLikeMQService = commentLikeMQService;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 点赞
        /// </summary>
        /// <param name="CommentId"></param>
        /// <returns></returns>
        [Authorize]
        public ResponseResult PraiseOperation(Guid Id, int LikeType)
        {
            try
            {
                var user = User.Identity.GetUserInfo();

                GiveLike giveLike = new GiveLike
                {
                    UserId = user.UserId,
                    LikeType = (LikeType)LikeType,
                    SourceId = Id
                };

                _likeMQService.SchoolCommentLike(giveLike);
                return ResponseResult.Success();
            }
            catch (Exception ex)
            {
                return ResponseResult.Failed(ex.Message);
            }
        }

        /// <summary>
        /// 图片上传
        /// </summary>
        /// <param name="imagesArr"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public ResponseResult AppUploadImager(List<string> images)
        {
            Guid Id = Guid.NewGuid();
            try
            {
                List<string> ImagerUrl = new List<string>();

                for (int i = 0; i < images.Count(); i++)
                {
                    string baseStr = images[i];

                    string[] sources = baseStr.Split(',');
                    //base64转字节
                    byte[] postData = Convert.FromBase64String(sources[1]);
                    //字节转image
                    Image img = GetImageExtHelper.byte2img(postData);
                    //获取图片扩展名
                    string ext = GetImageExtHelper.GetImageExt(img);
                    string commentImg = $"Comment/{Id}/{i}.{ext}";
                    //上传图片
                    int statusCode = UploadImager.UploadImagerByBase64(_setting.UploadImager + commentImg, postData);
                    if (statusCode == 200)
                    {
                        ImagerUrl.Add("/images/school_v3/" + commentImg);
                    }
                }
                return ResponseResult.Success(new { ImagerUrl, Id }, "图片上传成功");
            }
            catch (Exception)
            {
                return ResponseResult.Failed(new { Id });
            }
        }

        public IActionResult GetAdvs(int[] location, string callback)
        {
            AdvResponseModel response = new AdvResponseModel()
            {
                Advs = new List<AdvResponseModel.AdvOption>()
            };
            var cityId = Request.GetAvaliableCity();
            try
            {
                var clientType = Request.GetClientType();
                foreach (var locationId in location)
                {
                    var advs = _advertisingBaseService.GetAdvertising(locationId, cityId);
                    if (clientType == UA.Mobile)
                    {
                        //手机端，链接要加前缀
                        advs = advs.Select(s =>
                        {
                            s.Url = $"ischool://web?url={HttpUtility.UrlEncode(s.Url)}";
                            return s;
                        });
                    }
                    response.Advs.Add(new AdvResponseModel.AdvOption
                    {
                        Place = locationId,
                        Items = advs.ToList()
                    });
                }
                response.status = 0;
                response.msg = "success";
            }
            catch (Exception ex)
            {
                response.status = 1;
                response.msg = ex.Message;
            }
            if (string.IsNullOrEmpty(callback))
            {
                return Json(response);
            }
            else
            {
                //可能是jsonp
                var jsonResult = JsonConvert.SerializeObject(response);
                return Content($"{callback}({jsonResult})", "text/html");
            }
        }

        public ResponseResult CheckLogin()
        {
            var config = HttpContext.RequestServices.GetService(typeof(IConfiguration)) as IConfiguration;
            var userServiceUrl = config.GetSection("UserSystemConfig").GetValue<string>("ServerUrl");

            bool IsLogin = User.Identity.IsAuthenticated;
            return ResponseResult.Success(new { IsLogin, LoginUrl = userServiceUrl + "/login/login-pc.html?returnUrl=" });
        }

        /// <summary>
        /// 点评点赞
        /// </summary>
        /// <param name="DataId"></param>
        /// <returns></returns>
        [Authorize]
        [Description("点评点赞")]
        public ResponseResult LikeComment(Guid DataId, int LikeType)
        {
            try
            {
                var user = User.Identity.GetUserInfo();

                _mediator.Publish(new CommentLikeEvent(new GiveLike
                {
                    UserId = user.UserId,
                    LikeType = (LikeType)LikeType,
                    SourceId = DataId,
                    
                }));
                //_commentLikeMQService.SchoolCommentLike(giveLike);

                return ResponseResult.Success();
            }
            catch (Exception ex)
            {
                return ResponseResult.Failed(ex.Message);
            }
        }

        [Authorize]
        [Description("举报")]
        public ResponseResult ReportSubmit(List<string> images, string reason, string description, string evidenceURL, Guid id, byte type)
        {
            var user = User.Identity.GetUserInfo();

            var report = new Report() { UserID = user.UserId, DataID = id, Reason = reason, DataType = type, Description = description, EvidenceURL = evidenceURL };
            List<Report_Img> report_Imgs = new List<Report_Img>();
            if (images.Any())
            {
                for (int i = 0; i < images.Count(); i++)
                {
                    string baseStr = images[i];

                    string[] sources = baseStr.Split(',');
                    //base64转字节
                    byte[] postData = Convert.FromBase64String(sources[1]);
                    //字节转image
                    Image img = GetImageExtHelper.byte2img(postData);
                    //获取图片扩展名
                    string ext = GetImageExtHelper.GetImageExt(img);
                    Report_Img report_Img = new Report_Img();
                    report_Img.Report_Id = report.Id;

                    report_Img.url = "https://file.sxkid.com/Upload/Report?filename=" + report.Id + "/" + report_Img.Id + "." + ext;
                    //图片上传
                    var temp = UploadImager.UploadImagerByFeedback(report_Img.url, postData);
                    if (temp.status == 0)
                    {
                        report_Img.url = temp.url;
                    }
                    report_Img.bytes = postData;
                    report_Imgs.Add(report_Img);
                }
            }
            report.Report_Imgs = report_Imgs;
            if (!_reportService.AddReport(report))
            {
                return ResponseResult.Failed("举报失败");
            }

            //图片上传
            //foreach (var item in report_Imgs)
            //{
            //    UploadImager.UploadImagerByFeedback(item.url, item.bytes);
            //}
            return ResponseResult.Success("举报成功");
        }
    }
}