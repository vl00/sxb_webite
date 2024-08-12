using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using PMS.UserManage.Application.IServices;
using PMS.UserManage.Domain.Common;
using Sxb.UserCenter.Models.CommentViewModel;
using Sxb.UserCenter.Models.QuestionViewModel;
using Sxb.UserCenter.Response;
using Sxb.UserCenter.Utils;
using PMS.UserManage.Application.ModelDto.ModelVo;
using Sxb.UserCenter.Models.ArticleViewModel;
using Sxb.UserCenter.Utils.DtoToViewModel;
using Sxb.UserCenter.Models.LiveViewModel;
using ProductManagement.API.Http.Interface;
using ProductManagement.Framework.Foundation;
using static PMS.UserManage.Domain.Common.EnumSet;
using PMS.School.Application.IServices;
using PMS.School.Application.ModelDto;

namespace Sxb.UserCenter.Controllers
{
    /// <summary>
    /// 最近浏览
    /// </summary>
    public class ApiHistoryController : Base
    {

        public IsHost IsHost { get; }
        private IHistoryService _historyService { get; set; }
        private ISchoolService _schoolService { get; set; }
        private readonly ILiveServiceClient _liveServiceClient;
        public ApiHistoryController(IOptions<IsHost> _isHost, IHistoryService historyService,
            ISchoolService schoolService, ILiveServiceClient liveServiceClient)
        {
            IsHost = _isHost.Value;
            _historyService = historyService;
            _liveServiceClient = liveServiceClient;
            _schoolService = schoolService;
        }
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 最近浏览 点评
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public ResponseResult Comment(int page = 1)
        {
            string cookieStr = Request.Headers["Cookie"];
            var res = _historyService.GetCommentHistory(userID, cookieStr, page: page);
            var comment = JsonConvert.DeserializeObject<PMS.UserManage.Application.ModelDto.ModelVo.CommentVo.CommentReplyData>(res.data.ToString());

            List<CommentInfoViewModel> comments = new List<CommentInfoViewModel>();
            if (comment == null || comment.commentModels == null || !comment.commentModels.Any())
            {
                return ResponseResult.Success(comments);
            }

            foreach (var item in comment.commentModels)
            {
                CommentInfoViewModel model = new CommentInfoViewModel();
                model.AddTime = item.CreateTime;
                model.Content = item.Content;
                model.Id = item.Id;
                model.LikeTotal = item.LikeCount;
                model.ReplyTotal = item.ReplyCount;
                model.RumorRefuting = item.IsRumorRefuting;
                model.Images = item.Images;
                model.UserInfo = new Models.UserInfoViewModel.UserViewModel() { Id = item.UserId, HeadImage = item.UserHeadImage, UserName = item.UserName }.ToAnonyUserName(item.IsAnony);

                if (item.UserInfo != null)
                {
                    model.UserInfo.HeadImage = item.UserInfo.HeadImager;
                    model.UserInfo.UserName = item.UserInfo.NickName;
                }
                comments.Add(model);
            }
            return ResponseResult.Success(comments);
        }

        /// <summary>
        /// 最近浏览 提问
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public ResponseResult QA(int page = 1)
        {
            List<QuestionInfoViewModel> questions = new List<QuestionInfoViewModel>();
            string cookieStr = Request.Headers["Cookie"];
            var res = _historyService.GetQAHistory(userID, cookieStr, page: page);
            var question = JsonConvert.DeserializeObject<PMS.UserManage.Application.ModelDto.ModelVo.QuestionAnswerData>(res.data.ToString());
            if (question == null || question.questionModels == null || !question.questionModels.Any())
            {
                return ResponseResult.Success(questions);
            }

            foreach (var item in question.questionModels)
            {
                QuestionInfoViewModel model = new QuestionInfoViewModel();
                model.Id = item.Id;
                model.Content = item.QuestionContent;
                model.AddTime = item.QuestionCreateTime;
                questions.Add(model);
            }
            return ResponseResult.Success(questions);
        }

        /// <summary>
        /// 最近浏览 学校
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public async Task<ResponseResult> School(int page = 1)
        {
            var ids = _historyService.GetUserHistory(userID, 1, page);

            var result = ids.Count > 0 ? await _schoolService.ListExtSchoolByBranchIds(ids, latitude ?? 0, longitude ?? 0) : new List<SchoolExtFilterDto>();

            var school = result.Select(q => new SchoolModel
            {
                Sid = q.Sid,
                ExtId = q.ExtId,
                SchoolName = q.Name,
                City = q.CityCode,
                Area = q.AreaCode,
                CityName = q.City,
                AreaName = q.Area,
                Distance = q.Distance.ToString(),
                LodgingType = (int)q.LodgingType,
                LodgingReason = q.LodgingType.Description(),
                Tuition = q.Tuition,
                Tags = q.Tags,
                Score = q.Score,
                Grade = (byte)q.Grade,
                Type = (EnumSet.SchoolType)q.Type,
                CommentCount = q.CommentCount
            }).ToList();
            return ResponseResult.Success(school);
        }

        /// <summary>
        /// 最近浏览 文章
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public ResponseResult Article(int page = 1)
        {
            var res = _historyService.GetArticleHistory(userID, page: page);

            return ResponseResult.Success(ArticleDataToVoHelper.ArticleDataToViewModelHelper(res));
        }

        /// <summary>
        /// 最近浏览 直播
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public async Task<ResponseResult> Live(int page = 1)
        {
            var res = _historyService.GetUserHistory(userID, (int)MessageDataType.Lecture, page: page);

            List<LiveViewModel> liveViews = new List<LiveViewModel>();

            var result = await _liveServiceClient.QueryLectures(res, HttpContext.Request.GetAllCookies());
            if (result.Items != null && result.Items.Any())
            {
                liveViews = result.Items.Select(q => new LiveViewModel
                {
                    Id = q.Id,
                    Title = q.Subject,
                    FrontCover = q.HomeCover,
                    Time = ((long)q.Time).I2D().ConciseTime(),
                    ViewCount = q.Onlinecount,
                    LectorId = q.lector?.Id ?? Guid.Empty,
                    LectorName = q.lector?.Name,
                    LectorHeadImg = q.lector?.Headimgurl,
                }).ToList();
            }
            return ResponseResult.Success(liveViews);
        }

        [AllowAnonymous]
        public IActionResult AddHistory(Guid dataID, byte dataType)
        {
            PMS.UserManage.Domain.Common.RootModel json = new PMS.UserManage.Domain.Common.RootModel();
            try
            {
                if (dataID.Equals(Guid.Empty) || !_historyService.AddHistory(userID, dataID, dataType))
                {
                    json.status = 1;
                    json.errorDescription = "添加失败";
                }
            }
            catch (Exception ex)
            {
                json.status = 1;
                json.errorDescription = ex.ToString();
            }
            return Json(json);
        }

        /// <summary>
        /// 清空最近浏览
        /// </summary>
        /// <param name="Ids"></param>
        /// <returns></returns>
        [HttpPost]
        public ResponseResult ClearHistory([FromBody] List<Guid> Ids)
        {
            if (!Ids.Any())
            {
                return ResponseResult.Success();
            }

            Guid UserId = Guid.Empty;
            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity.GetUserInfo();
                UserId = user.UserId;
            }
            //更改数据状态
            bool res = _historyService.ChangeHistoryState(Ids, UserId);
            return ResponseResult.Success(res);
        }

        /// <summary>
        /// 清空最近浏览
        /// </summary>
        /// <param name="type">0：文章 1：学校学部 2：问答 3：点评 4:直播</param>
        /// <returns></returns>
        [HttpPost]
        public ResponseResult ClearAllHistory(int type)
        {
            Guid UserId = Guid.Empty;
            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity.GetUserInfo();
                UserId = user.UserId;
            }
            //更改数据状态
            bool res = _historyService.ClearAllHistory(UserId, type);
            return ResponseResult.Success(res);
        }
    }
}