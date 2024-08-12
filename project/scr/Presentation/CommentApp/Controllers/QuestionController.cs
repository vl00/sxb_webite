using AutoMapper;
using CommentApp.Models.School;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using PMS.CommentsManage.Application.IServices;
using PMS.CommentsManage.Application.ModelDto;
using PMS.CommentsManage.Domain.Common;
using PMS.CommentsManage.Domain.Entities;
using PMS.CommentsManage.Domain.Entities.Total;
using PMS.Infrastructure.Application.IService;
using PMS.Infrastructure.Application.ModelDto;
using PMS.School.Application.IServices;
using PMS.School.Application.ModelDto;
using PMS.UserManage.Application.IServices;
using PMS.UserManage.Application.ModelDto;
using PMS.UserManage.Domain.Common;
using ProductManagement.API.Aliyun;
using ProductManagement.API.Http.Interface;
using ProductManagement.Framework.Cache.Redis;
using ProductManagement.Framework.Foundation;
using ProductManagement.Infrastructure.Exceptions;
using ProductManagement.Infrastructure.Toolibrary;
using Sxb.Api.RequestModel.RequestEnum;
using Sxb.Api.RequestModel.RequestOption;
using Sxb.Web.Common;
using Sxb.Web.Models;
using Sxb.Web.Models.Answer;
using Sxb.Web.Models.Question;
using Sxb.Web.Models.Replay;
using Sxb.Web.Models.User;
using Sxb.Web.Response;
using Sxb.Web.Utils;
using Sxb.Web.ViewModels.Api;
using Sxb.Web.ViewModels.Question;
using Sxb.Web.ViewModels.School;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using static PMS.UserManage.Domain.Common.EnumSet;

namespace Sxb.Web.Controllers
{
    public class QuestionController : BaseController
    {
        private readonly IMessageService _message;

        private readonly IMessageService _inviteStatus;

        private readonly ImageSetting _setting;
        private readonly IQuestionInfoService _questionInfo;
        private readonly ISchoolService _schoolService;
        private readonly ISchoolInfoService _schoolInfoService;
        private readonly ISchoolCommentService _commentService;
        private readonly ISchoolImageService _imageService;
        private readonly IQuestionsAnswersInfoService _questionsAnswers;
        private readonly IUserService _userService;
        private readonly ICollectionService _collectionService;
        private readonly IEasyRedisClient _easyRedisClient;
        readonly ICityInfoService _cityService;

        private readonly IGiveLikeService _likeService;

        private readonly IUserServiceClient _userMessage;
        //管理员
        private readonly IPartTimeJobAdminService _partTimeJobAdmin;

        private readonly IUserServiceClient _userServiceClient;

        private readonly IUserGrantAuthService _userGrantAuthService;

        private IPartTimeJobAdminRolereService _adminRolereService;
        private ISettlementAmountMoneyService _amountMoneyService;

        ISchService _SchService;

        private readonly IMapper _mapper;

        private readonly IText _text;
        public QuestionController(IEasyRedisClient easyRedisClient, ICityInfoService cityCodeService, IOptions<ImageSetting> set,
            IQuestionsAnswersInfoService questionsAnswers, ISchoolImageService imageService, IUserService userService,
            ISchoolService schoolService, IQuestionInfoService questionInfo, ISchoolCommentService commentService,
            IUserServiceClient userServiceClient, ISchoolInfoService schoolInfoService, ICollectionService collectionService,
            IPartTimeJobAdminService partTimeJobAdmin, IUserGrantAuthService userGrantAuthService,
            IUserServiceClient userMessage, IPartTimeJobAdminRolereService adminRolereService,
            ISettlementAmountMoneyService amountMoneyService, IGiveLikeService likeService, IText text, IMessageService message,
            IMessageService inviteStatus, IMapper mapper, ISchService schService)
        {
            _SchService = schService;
            _inviteStatus = inviteStatus;
            _easyRedisClient = easyRedisClient;
            _cityService = cityCodeService;
            _schoolInfoService = schoolInfoService;
            _userMessage = userMessage;
            _questionsAnswers = questionsAnswers;
            _setting = set.Value;
            _questionInfo = questionInfo;
            _schoolService = schoolService;
            _commentService = commentService;
            _imageService = imageService;
            _userService = userService;
            _collectionService = collectionService;
            _partTimeJobAdmin = partTimeJobAdmin;

            _userServiceClient = userServiceClient;

            _likeService = likeService;

            _userGrantAuthService = userGrantAuthService;

            _adminRolereService = adminRolereService;
            _amountMoneyService = amountMoneyService;

            _message = message;

            _text = text;

            _mapper = mapper;
        }

        [Description("学校问答页")]
        [Route("/{controller}/{action}/{Id}")]
        [Route("/{controller}/{action}-{schoolNo}/")]
        [Route("/{controller}/{action}-{schoolNo}/{tab}")]
        public async Task<IActionResult> School(Guid Id, int queryQuestion = 1, SelectedQuestionOrder selectedQuestion = SelectedQuestionOrder.Intelligence,
            string schoolNo = default)
        {
            //检测当前用户身份 （true：校方用户 |false：普通用户）
            ViewBag.isSchoolRole = false;
            //检测是否关注该学校
            ViewBag.isCollected = false;

            Guid userId = Guid.Empty;
            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity.GetUserInfo();
                //检测当前用户身份 （true：校方用户 |false：普通用户）
                ViewBag.isSchoolRole = user.Role.Contains((int)PMS.UserManage.Domain.Common.UserRole.School);
                userId = user.UserId;


                //检测改用户是否关注该学校
                var checkCollectedSchool = await _userServiceClient.IsCollected(new ProductManagement.API.Http.Model.IsCollected() { userId = userId, dataID = Id });
                ViewBag.isCollected = checkCollectedSchool.iscollected;
            }

            if (!string.IsNullOrWhiteSpace(schoolNo))
            {
                var ext = await _SchService.GetSchextSimpleInfoViaShortNo(schoolNo);
                if (ext != null)
                {
                    Id = ext.Eid;
                }
                else
                {
                    return new ExtNotFoundViewResult();
                }
            }

            var school = _schoolInfoService.QueryESchoolInfo(Id);

            if (school == null)
            {
                throw new NotFoundException();
            }

            _ = _userServiceClient.AddHistory(new ProductManagement.API.Http.Model.AddHistory() { dataID = school.SchoolSectionId, dataType = MessageDataType.School, cookies = HttpContext.Request.GetAllCookies() });

            var extension = _mapper.Map<SchoolInfoDto, SchoolExtensionVo>(school);

            ViewBag.School = extension;
            ViewBag.QuestionTotal = _questionInfo.TotalQuestion(Id);
            var schoolTotal = _mapper.Map<List<SchoolQuestionTotal>, List<SchoolQuesionTotalVo>>(_questionInfo.CurrentQuestionTotalBySchoolId(school.SchoolId));

            var AllExtension = _schoolService.GetSchoolExtName(school.SchoolId);
            for (int i = 0; i < AllExtension.Count(); i++)
            {
                var item = schoolTotal.Where(x => x.SchoolSectionId == AllExtension[i].Value).FirstOrDefault();
                int currentIndex = schoolTotal.IndexOf(item);
                if (item == null)
                {
                    if (AllExtension[i].Key != null)
                    {
                        schoolTotal.Add(new SchoolQuesionTotalVo() { Name = AllExtension[i].Key, SchoolSectionId = AllExtension[i].Value, TotalType = QueryCondition.Other.Description() });
                    }
                }
                else
                {
                    if (AllExtension[i].Key != null)
                    {
                        item.Name = AllExtension[i].Key;
                        schoolTotal[currentIndex] = item;
                    }
                }
            }

            //将其他分部类型学校进行再次排序
            for (int i = 8; i < schoolTotal.Count(); i++)
            {
                schoolTotal[i].TotalTypeNumber += 1;
            }

            //排除所有下架学校标签筛选
            schoolTotal.Where(x => x.Name == null && x.SchoolSectionId != Guid.Empty)?.ToList().ForEach(x =>
            {
                schoolTotal.Remove(x);
            });
            ViewBag.CommentTotal = schoolTotal;

            if (queryQuestion == 1)
            {
                var HotQuestion = _questionInfo.GetHotQuestionInfoBySchoolId(Id, userId, 1, 3, selectedQuestion);
                var hotQuestionList = new List<QuestionVo>();
                if (HotQuestion != null)
                {
                    hotQuestionList = _mapper.Map<List<QuestionDto>, List<QuestionVo>>(HotQuestion);

                    var UserIds = new List<Guid>();
                    UserIds.AddRange(hotQuestionList.GroupBy(q => q.UserId).Select(p => p.Key).ToList());
                    hotQuestionList.Where(q => q.Answer != null).ToList().ForEach(p => { UserIds.AddRange(p.Answer.Select(q => q.UserId)); });
                    hotQuestionList.Where(q => q.Answer != null).ToList().ForEach(p => { UserIds.AddRange(p.Answer.Where(q => q.ParentId != null).Select(q => (Guid)q.ParentId)); });
                    var Users = _userService.ListUserInfo(UserIds.GroupBy(q => q).Select(p => p.Key).ToList());

                    var extIds = hotQuestionList.GroupBy(q => q.SchoolSectionId).Select(q => q.Key).ToList();
                    var schoolInfos = _schoolInfoService.GetSchoolSectionQaByIds(extIds);

                    hotQuestionList.ForEach(x =>
                    {
                        x.QuestionContent = System.Text.RegularExpressions.Regex.Replace(x.QuestionContent, @"\s", "");
                        var schoolInfo = schoolInfos.FirstOrDefault(q => q.SchoolSectionId == x.SchoolSectionId);
                        x.School = _mapper.Map<SchoolInfoQaDto, QuestionVo.SchoolInfo>(schoolInfo);

                        var images = x.Images;
                        for (int i = 0; i < images.Count; i++)
                        {
                            images[i] = _setting.QueryImager + images[i];
                        }
                        x.Images = images;

                        if (x.Answer != null && x.Answer.Count > 0)
                        {
                            x.Answer[0].UserInfo = _mapper.Map<UserInfoVo>(Users.FirstOrDefault(q => q.Id == x.Answer[0].UserId)).ToAnonyUserName(x.Answer[0].IsAnony);
                        }

                        x.No = x.No.ToLower();
                    });
                }
                ViewBag.QuestionHot = hotQuestionList;
            }
            //保存当前选项卡选中值
            ViewBag.SelectedTab = (int)queryQuestion;

            ViewBag.SchoolId = school;
            return View();
        }

        [Description("问答详情页")]
        [Route("question/{No}.html")]
        [Route("question/detail/{QuestionId}")]
        public async Task<IActionResult> Detail(Guid QuestionId, string No)
        {
            //检测当前用户身份 （true：校方用户 |false：普通用户）
            ViewBag.isSchoolRole = false;
            //检测是否关注该问题
            ViewBag.isCollected = false;
            Guid userId = Guid.Empty;

            //问题详情
            QuestionDto question = new QuestionDto();
            long numberId = 0;

            if (No != null)
            {
                numberId = UrlShortIdUtil.Base322Long(No);
            }

            if (numberId > 0)
            {
                question = await _questionInfo.QuestionDetailByNo(numberId, userId);
            }
            else
            {
                question = _questionInfo.QuestionDetail(QuestionId, userId);
            }

            //问题详情
            //var question = await _questionInfo.QuestionDetail(QuestionId, userId);
            if (question == null)
            {
                throw new NotFoundException();
            }

            QuestionId = question.Id;

            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity.GetUserInfo();
                //检测当前用户身份 （true：校方用户 |false：普通用户）
                ViewBag.isSchoolRole = user.Role.Contains((int)PMS.UserManage.Domain.Common.UserRole.School);
                userId = user.UserId;


                //检测改用户是否关注该问题
                var isCollected =  _collectionService.IsCollected(userId, QuestionId, CollectionDataType.QA);
                ViewBag.isCollected = isCollected;
            }

            QuestionId = question.Id;

            _ = _userServiceClient.AddHistory(new ProductManagement.API.Http.Model.AddHistory() { dataID = question.Id, dataType = MessageDataType.Question, cookies = HttpContext.Request.GetAllCookies() });

            _questionInfo.UpdateQuestionViewCount(QuestionId);

            var Hot = _questionsAnswers.GetAnswerInfoByQuestionId(QuestionId, userId, 1, 3);
            var newAnswer = _questionsAnswers.GetNewAnswerInfoByQuestionId(QuestionId, userId, 1, 3, out int total);

            var UserIds = new List<Guid>();
            UserIds.AddRange(Hot.GroupBy(q => q.UserId).Select(p => p.Key).ToList());
            UserIds.AddRange(newAnswer.GroupBy(q => q.UserId).Select(p => p.Key).ToList());
            UserIds.Add(question.UserId);
            UserIds.Add(userId);
            var Users = _userService.ListUserInfo(UserIds.GroupBy(q => q).Select(p => p.Key).ToList());

            //问题提问者
            ViewBag.QuestionUser = Users.Where(x => x.Id == userId).FirstOrDefault();

            question.Images.ForEach(x => x = _setting.QueryImager + x);

            var images = question.Images;
            for (int i = 0; i < images.Count; i++)
            {
                images[i] = _setting.QueryImager + images[i];
            }
            question.Images = images;

            var questionRes = _mapper.Map<QuestionDto, QuestionVo>(question);
            questionRes.UserInfo = _mapper.Map<UserInfoVo>(Users.FirstOrDefault(q => q.Id == questionRes.UserId));
            //问题详情
            ViewBag.Question = questionRes;

            //该分部下的问题总数
            ViewBag.QuestionTotal = _questionInfo.TotalQuestion(question.SchoolSectionId);

            //学校详情信息
            var school = _schoolService.GetSchoolByIds(question.SchoolSectionId.ToString()).FirstOrDefault();
            if (school == null)
            {
                throw new NotFoundException();
            }


            var extension = _mapper.Map<SchoolDto, SchoolExtensionVo>(school);

            ViewBag.School = extension;

            var hotList = _mapper.Map<List<AnswerInfoDto>, List<AnswerInfoVo>>(Hot);

            for (int i = 0; i < hotList.Count(); i++)
            {
                hotList[i].UserInfo = _mapper.Map<UserInfoVo>(Users.FirstOrDefault(q => q.Id == hotList[i].UserId)).ToAnonyUserName(hotList[i].IsAnony);
            }

            //问题回答
            ViewBag.HotAnswers = hotList;


            var newAnswerList = _mapper.Map<List<AnswerInfoDto>, List<AnswerInfoVo>>(newAnswer);
            if (newAnswerList.Any())
            {
                for (int i = 0; i < newAnswerList.Count(); i++)
                {
                    newAnswerList[i].UserInfo = _mapper.Map<UserInfoVo>(Users.FirstOrDefault(q => q.Id == newAnswerList[i].UserId)).ToAnonyUserName(newAnswerList[i].IsAnony);
                }
                //最新回答
                ViewBag.NewAnswers = newAnswerList;
            }

            return View();
        }


        [Description("学校问答页")]
        public IActionResult Index(Guid SchoolId, int queryQuestion = 1, SelectedQuestionOrder selectedQuestion = SelectedQuestionOrder.Intelligence)
        {
            if (SchoolId == Guid.Empty)
            {
                throw new NotFoundException();
            }
            else
            {
                var redirectUrl = $"/question/school/{SchoolId}";
                Response.Redirect(redirectUrl);
                Response.StatusCode = 301;
                return Json(new { });
            }

            //#region

            ////检测当前用户身份 （true：校方用户 |false：普通用户）
            //ViewBag.isSchoolRole = false;
            ////检测是否关注该学校
            //ViewBag.isCollected = false;

            //_=_userServiceClient.AddHistory(new ProductManagement.API.Http.Model.AddHistory() { dataID = SchoolId, dataType = ProductManagement.API.Http.RequestCommon.MessageDataType.SchoolSection, cookies = HttpContext.Request.GetAllCookies() });

            //Guid userId = Guid.Empty;
            //if (User.Identity.IsAuthenticated)
            //{
            //    var user = User.Identity.GetUserInfo();
            //    //检测当前用户身份 （true：校方用户 |false：普通用户）
            //    ViewBag.isSchoolRole = user.Role.Contains((int)PMS.UserManage.Domain.Common.UserRole.School);
            //    userId = user.UserId;


            //    //检测改用户是否关注该学校
            //    var checkCollectedSchool = await _userServiceClient.IsCollected(new ProductManagement.API.Http.Model.IsCollected() { userId = userId, dataID = SchoolId });
            //    ViewBag.isCollected = checkCollectedSchool.iscollected;
            //}

            //var school = _schoolInfoService.QueryESchoolInfo(SchoolId);

            //if (school == null)
            //{
            //    throw new NotFoundException();
            //}

            //var extension = _mapper.Map<SchoolInfoDto, SchoolExtensionVo>(school);

            //ViewBag.School = extension;
            //ViewBag.QuestionTotal = _questionInfo.TotalQuestion(SchoolId);
            //var schoolTotal = _mapper.Map<List<SchoolQuestionTotal>, List<SchoolQuesionTotalVo>>(_questionInfo.CurrentQuestionTotalBySchoolId(school.SchoolId));

            //var AllExtension = _schoolService.GetSchoolExtName(school.SchoolId);
            //for (int i = 0; i < AllExtension.Count(); i++)
            //{
            //    var item = schoolTotal.Where(x => x.SchoolSectionId == AllExtension[i].Value).FirstOrDefault();
            //    int currentIndex = schoolTotal.IndexOf(item);
            //    if (item == null)
            //    {
            //        if (AllExtension[i].Key != null)
            //        {
            //            schoolTotal.Add(new SchoolQuesionTotalVo() { Name = AllExtension[i].Key, SchoolSectionId = AllExtension[i].Value, TotalType = QueryCondition.Other.Description() });
            //        }
            //    }
            //    else
            //    {
            //        if (AllExtension[i].Key != null)
            //        {
            //            item.Name = AllExtension[i].Key;
            //            schoolTotal[currentIndex] = item;
            //        }
            //    }
            //}


            ////将其他分部类型学校进行再次排序
            //for (int i = 8; i < schoolTotal.Count(); i++)
            //{
            //    schoolTotal[i].TotalTypeNumber += 1;
            //}

            ////排除所有下架学校标签筛选
            //schoolTotal.Where(x => x.Name == null && x.SchoolSectionId != Guid.Empty)?.ToList().ForEach(x => {
            //    schoolTotal.Remove(x);
            //});
            //ViewBag.CommentTotal = schoolTotal;

            //if (queryQuestion == 1)
            //{
            //    var HotQuestion =  _questionInfo.GetHotQuestionInfoBySchoolId(SchoolId, userId, 1, 3, selectedQuestion);
            //    var hotQuestionList = new List<QuestionVo>();
            //    if (HotQuestion != null)
            //    {
            //        hotQuestionList = _mapper.Map<List<QuestionDto>, List<QuestionVo>>(HotQuestion);

            //        var UserIds = new List<Guid>();
            //        UserIds.AddRange(hotQuestionList.GroupBy(q => q.UserId).Select(p => p.Key).ToList());
            //        hotQuestionList.Where(q => q.Answer != null).ToList().ForEach(p => { UserIds.AddRange(p.Answer.Select(q => q.UserId)); });
            //        hotQuestionList.Where(q => q.Answer != null).ToList().ForEach(p => { UserIds.AddRange(p.Answer.Where(q => q.ParentId != null).Select(q => (Guid)q.ParentId)); });
            //        var Users = _userService.ListUserInfo(UserIds.GroupBy(q => q).Select(p => p.Key).ToList());

            //        var extIds = hotQuestionList.GroupBy(q => q.SchoolSectionId).Select(q => q.Key).ToList();
            //        var schoolInfos = _schoolInfoService.GetSchoolSectionQaByIds(extIds);

            //        hotQuestionList.ForEach(x =>
            //        {
            //            x.QuestionContent = System.Text.RegularExpressions.Regex.Replace(x.QuestionContent, @"\s", "");
            //            var schoolInfo = schoolInfos.FirstOrDefault(q => q.SchoolSectionId == x.SchoolSectionId);
            //            x.School = _mapper.Map<SchoolInfoQaDto, QuestionVo.SchoolInfo>(schoolInfo);

            //            var images = x.Images;
            //            for (int i = 0; i < images.Count; i++)
            //            {
            //                images[i] = _setting.QueryImager + images[i];
            //            }
            //            x.Images = images;

            //            if (x.Answer != null && x.Answer.Count > 0)
            //            {
            //                x.Answer[0].UserInfo = _mapper.Map<UserInfoVo>(Users.FirstOrDefault(q => q.Id == x.Answer[0].UserId)).ToAnonyUserName(x.Answer[0].IsAnony);
            //            }
            //        });
            //    }
            //    ViewBag.QuestionHot = hotQuestionList;
            //}
            ////保存当前选项卡选中值
            //ViewBag.SelectedTab = (int)queryQuestion;

            //ViewBag.SchoolId = school;
            //return View();

            //#endregion
        }

        [Authorize]
        [Description("提交问题页")]
        public async Task<IActionResult> write(Guid eid)
        {

            Guid userId = Guid.Empty;
            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity.GetUserInfo();
                userId = user.UserId;
            }
            var adCodes = await _cityService.IntiAdCodes();
            //获取该学校所有的分部信息
            var school = _schoolInfoService.GetSchoolName(new List<Guid>() { eid });

            //var currentSchool = _schoolService.GetSchoolByIds(currentId.ToString()).FirstOrDefault();
            var currentSchool = school.Where(x => x.SchoolSectionId == eid).FirstOrDefault();
            if (currentSchool == null)
            {
                throw new NotFoundException();
            }

            //ViewBag.CurrentSchool = new SchoolSwitch() { Id = currentSchool.SchoolSectionId, Name = currentSchool.SchoolName, City = adCodes.SelectMany(x => x.CityCodes).FirstOrDefault(p => p.Id == currentSchool.City).Name };
            ViewBag.CurrentSchool = new SchoolSwitch() { Id = currentSchool.SchoolSectionId, Name = currentSchool.SchoolName };

            var allSchoolBranch = school.Where(x => x.SchoolSectionId != eid)?.ToList();
            ViewBag.AllSchoolBranch = allSchoolBranch == null ? new List<SchoolSwitch>() : allSchoolBranch.Select(x => new SchoolSwitch()
            {
                Id = x.SchoolSectionId,
                Name = x.SchoolName,
                //City = adCodes.SelectMany(c => c.CityCodes).FirstOrDefault(p => p.Id == x.City).Name
            }).ToList();
            ViewBag.SchoolId = currentSchool.SchoolId;

            List<AreaDto> history = new List<AreaDto>();

            if (userId != default(Guid))
            {
                string Key = $"SearchCity:{userId}";
                var historyCity = await _easyRedisClient.SortedSetRangeByRankAsync<AreaDto>(Key, 0, 10, StackExchange.Redis.Order.Descending);
                history.AddRange(historyCity.ToList());
            }

            //获取当前用户所在城市
            var localCityCode = Request.GetLocalCity();

            //将数据保存在前台
            ViewBag.LocalCity = adCodes.SelectMany(p => p.CityCodes).FirstOrDefault(p => p.Id == localCityCode);

            ViewBag.HotCity = JsonConvert.SerializeObject(await _cityService.GetHotCity());

            ViewBag.History = JsonConvert.SerializeObject(history);

            ViewBag.SchoolSectionId = eid;
            //城市编码
            ViewBag.AllCity = JsonConvert.SerializeObject(adCodes);

            ViewBag.IsWriteAuth = _userGrantAuthService.IsGrantAuth(userId);

            return View();
        }

        [Authorize]
        [Description("提交问题页")]
        public IActionResult CommentQuestionView(Guid currentId, Guid SchoolId)
        {
            var redirectUrl = $"/question/write?eid={currentId}";
            Response.Redirect(redirectUrl);
            Response.StatusCode = 301;
            return Json(new { });

            //Guid userId = Guid.Empty;
            //if (User.Identity.IsAuthenticated)
            //{
            //    var user = User.Identity.GetUserInfo();
            //    userId = user.UserId;
            //}
            //var adCodes = await _cityService.IntiAdCodes();
            ////获取该学校所有的分部信息
            //var school = _schoolService.GetAllSchoolBranch(SchoolId);

            ////var currentSchool = _schoolService.GetSchoolByIds(currentId.ToString()).FirstOrDefault();
            //var currentSchool = school.Where(x => x.Id == currentId).FirstOrDefault();
            //if (currentSchool == null)
            //{
            //    throw new NotFoundException();
            //}

            //ViewBag.CurrentSchool = new SchoolSwitch() { Id = currentSchool.Id, Name = currentSchool.Name, City = adCodes.SelectMany(x => x.CityCodes).FirstOrDefault(p => p.Id == currentSchool.City).Name };

            //var allSchoolBranch = school.Where(x => x.Id != currentId)?.ToList();
            //ViewBag.AllSchoolBranch = allSchoolBranch == null ? new List<SchoolSwitch>() : allSchoolBranch.Select(x => new SchoolSwitch()
            //{
            //    Id = x.Id,
            //    Name = x.Name,
            //    City = adCodes.SelectMany(c => c.CityCodes).FirstOrDefault(p => p.Id == x.City).Name
            //}).ToList();
            //ViewBag.SchoolId = SchoolId;

            //List<AreaDto> history = new List<AreaDto>();

            //if (userId != default(Guid))
            //{
            //    string Key = $"SearchCity:{userId}";
            //    var historyCity = await _easyRedisClient.SortedSetRangeByRankAsync<AreaDto>(Key, 0, 10, StackExchange.Redis.Order.Descending);
            //    history.AddRange(historyCity.ToList());
            //}

            ////获取当前用户所在城市
            //var localCityCode = Request.GetLocalCity();

            ////将数据保存在前台
            //ViewBag.LocalCity = adCodes.SelectMany(p => p.CityCodes).FirstOrDefault(p => p.Id == localCityCode);

            //ViewBag.HotCity = JsonConvert.SerializeObject(await _cityService.GetHotCity());

            //ViewBag.History = JsonConvert.SerializeObject(history);

            //ViewBag.SchoolSectionId = currentId;
            ////城市编码
            //ViewBag.AllCity = JsonConvert.SerializeObject(adCodes);

            //ViewBag.IsWriteAuth = _userGrantAuthService.IsGrantAuth(userId);

            //return View();
        }

        [Description("问答详情页")]
        public IActionResult QuestionDetail(Guid QuestionId)
        {
            if (QuestionId == null || QuestionId == Guid.Empty)
            {
                throw new NotFoundException();
            }
            else
            {
                var redirectUrl = $"/question/detail/{QuestionId}";
                Response.Redirect(redirectUrl);
                Response.StatusCode = 301;
                return Json(new { });
            }

            //#region
            ////检测当前用户身份 （true：校方用户 |false：普通用户）
            //ViewBag.isSchoolRole = false;
            ////检测是否关注该问题
            //ViewBag.isCollected = false;
            //Guid userId = Guid.Empty;

            //_=_userServiceClient.AddHistory(new ProductManagement.API.Http.Model.AddHistory() { dataID = QuestionId, dataType = ProductManagement.API.Http.RequestCommon.MessageDataType.Question, cookies = HttpContext.Request.GetAllCookies() });

            //if (User.Identity.IsAuthenticated)
            //{
            //    var user = User.Identity.GetUserInfo();
            //    //检测当前用户身份 （true：校方用户 |false：普通用户）
            //    ViewBag.isSchoolRole = user.Role.Contains((int)PMS.UserManage.Domain.Common.UserRole.School);
            //    userId = user.UserId;


            //    //检测改用户是否关注该问题
            //    var checkCollectedSchool = await _userServiceClient.IsCollected(new ProductManagement.API.Http.Model.IsCollected() { userId = userId, dataID = QuestionId });
            //    ViewBag.isCollected = checkCollectedSchool.iscollected;
            //}

            ////问题详情
            //var question =  _questionInfo.QuestionDetail(QuestionId, userId);
            //if (question == null)
            //{
            //    throw new NotFoundException();
            //}
            //_questionInfo.UpdateQuestionViewCount(QuestionId);

            //var Hot = _questionsAnswers.GetAnswerInfoByQuestionId(QuestionId, userId, 1, 3);
            //var newAnswer = _questionsAnswers.GetNewAnswerInfoByQuestionId(QuestionId, userId, 1, 3, out int total);

            //var UserIds = new List<Guid>();
            //UserIds.AddRange(Hot.GroupBy(q => q.UserId).Select(p => p.Key).ToList());
            //UserIds.AddRange(newAnswer.GroupBy(q => q.UserId).Select(p => p.Key).ToList());
            //UserIds.Add(question.UserId);
            //UserIds.Add(userId);
            //var Users = _userService.ListUserInfo(UserIds.GroupBy(q => q).Select(p => p.Key).ToList());

            ////问题提问者
            //ViewBag.QuestionUser = Users.Where(x => x.Id == userId).FirstOrDefault();

            //question.Images.ForEach(x => x = _setting.QueryImager + x);

            //var images = question.Images;
            //for (int i = 0; i < images.Count; i++)
            //{
            //    images[i] = _setting.QueryImager + images[i];
            //}
            //question.Images = images;

            //var questionRes = _mapper.Map<QuestionDto, QuestionVo>(question);
            //questionRes.UserInfo = _mapper.Map<UserInfoVo>(Users.FirstOrDefault(q => q.Id == questionRes.UserId));
            ////问题详情
            //ViewBag.Question = questionRes;

            ////该分部下的问题总数
            //ViewBag.QuestionTotal = _questionInfo.TotalQuestion(question.SchoolSectionId);

            ////学校详情信息
            //var school = _schoolService.GetSchoolByIds(question.SchoolSectionId.ToString()).FirstOrDefault();
            //if (school == null)
            //{
            //    throw new NotFoundException();
            //}


            //var extension = _mapper.Map<SchoolDto, SchoolExtensionVo>(school);

            //ViewBag.School = extension;

            //var hotList = _mapper.Map<List<AnswerInfoDto>, List<AnswerInfoVo>>(Hot);

            //for (int i = 0; i < hotList.Count(); i++)
            //{
            //    hotList[i].UserInfo = _mapper.Map<UserInfoVo>(Users.FirstOrDefault(q => q.Id == hotList[i].UserId)).ToAnonyUserName(hotList[i].IsAnony);
            //}

            ////问题回答
            //ViewBag.HotAnswers = hotList;


            //var newAnswerList = _mapper.Map<List<AnswerInfoDto>, List<AnswerInfoVo>>(newAnswer);
            //if (newAnswerList.Any())
            //{
            //    for (int i = 0; i < newAnswerList.Count(); i++)
            //    {
            //        newAnswerList[i].UserInfo = _mapper.Map<UserInfoVo>(Users.FirstOrDefault(q => q.Id == newAnswerList[i].UserId)).ToAnonyUserName(newAnswerList[i].IsAnony);
            //    }
            //    //最新回答
            //    ViewBag.NewAnswers = newAnswerList;
            //}

            //return View();
            //#endregion
        }

        /// <summary>
        /// 提问提交
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [Sxb.Web.Authentication.Attribute.BindMobile]
        [Description("提交问题")]
        public ResponseResult CommitQuestion(QuestionInfo question, Guid questionId, List<string> questionImages)
        {
            try
            {
                var user = User.Identity.GetUserInfo();
                //QuestionInfo question = JsonConvert.DeserializeObject<QuestionInfo>(HttpContext.Request.Form["question"]);

                //if (question.SchoolId == default(Guid))
                //{
                //    var SchoolInfo = _schoolService.GetSchoolByIds(question.SchoolSectionId.ToString()).FirstOrDefault();
                //    question.SchoolId = SchoolInfo.SchoolId;
                //}

                var contentCheck = _text.GarbageCheck(new ProductManagement.API.Aliyun.Model.GarbageCheckRequest()
                {
                    scenes = new[] { "antispam" },
                    tasks = new List<ProductManagement.API.Aliyun.Model.GarbageCheckRequest.Task>() {
                      new ProductManagement.API.Aliyun.Model.GarbageCheckRequest.Task(){
                       content=question.Content,
                         dataId = Guid.NewGuid().ToString()
                      }
                      }
                }).Result;

                if (contentCheck.data.FirstOrDefault().results.FirstOrDefault().suggestion == "block")
                {
                    return ResponseResult.Failed("您发布的内容包含敏感词，<br>请重新编辑后再发布。");
                }

                question.Id = questionId;

                //string image = HttpContext.Request.Form["imagers"].ToString();
                //List<string> questionImages = new List<string>();
                //if (image != "")
                //{
                //    questionImages.AddRange(image.Split(",").ToList());
                //}

                List<SchoolImage> imgs = new List<SchoolImage>();
                //图片上传
                if (questionImages.Count() > 0)
                {
                    //标记该点评有上传过图片
                    question.IsHaveImagers = true;

                    //需要上传图片
                    imgs = new List<SchoolImage>();

                    for (int i = 0; i < questionImages.Count(); i++)
                    {
                        SchoolImage commentImg = new SchoolImage();
                        commentImg.DataSourcetId = question.Id;
                        commentImg.ImageType = ImageType.Question;
                        commentImg.ImageUrl = questionImages[i];

                        //图片上传成功，提交数据库
                        imgs.Add(commentImg);

                    }
                }

                question.UserId = user.UserId;
                question.PostUserRole = user.Role.Length > 0 ? (UserRole)user.Role[0] : UserRole.Member;

                if (question.SchoolId == default(Guid))
                {
                    question.SchoolId = _schoolInfoService.GetSchoolName(new List<Guid>() { question.SchoolSectionId }).FirstOrDefault().SchoolId;
                }

                _questionInfo.AddQuestion(question);

                //所有邀请我提问这个学校的消息, 全部设为已读,已处理
                _message.UpdateMessageHandled(true, MessageType.InviteQuestion, MessageDataType.School, user.UserId, default, new List<Guid>() { question.SchoolSectionId });

                if (imgs.Count != 0)
                {
                    foreach (var img in imgs)
                    {
                        _imageService.AddSchoolImage(img);
                    }
                }

                SuccessCommentViewModel successQuestion = new SuccessCommentViewModel
                {
                    DataId = question.Id,
                    SchoolId = question.SchoolSectionId
                };

                return ResponseResult.Success(successQuestion);
            }
            catch (Exception ex)
            {
                return ResponseResult.Failed(ex.Message);
            }
        }

        [Authorize]
        [Description("提交问题成功页")]
        public IActionResult QuestionSuccess(Guid SchoolId, Guid QuestionId, bool isSuccessQuestion)
        {
            //var school = _schoolInfoService.QuerySchoolInfo(SchoolId).Result;
            string SchoolName = _schoolInfoService.GetSchoolName(new List<Guid>() { SchoolId }).FirstOrDefault().SchoolName;

            ViewBag.SchoolId = SchoolId;

            ViewBag.isSuccessQuestion = isSuccessQuestion;
            //检测当前用户身份 （true：校方用户 |false：普通用户）
            ViewBag.isSchoolRole = false;
            Guid userId = default(Guid);
            var dto = _questionInfo.GetQuestionById(QuestionId, userId);

            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity.GetUserInfo();
                userId = user.UserId;
                //检测当前用户身份 （true：校方用户 |false：普通用户）
                ViewBag.isSchoolRole = user.Role.Contains((int)PMS.UserManage.Domain.Common.UserRole.School);
                ViewBag.ShareTitle = $"{user.Name} 邀请你回答：{dto.QuestionContent}";
            }

            var question = _questionInfo.GetQuestionById(QuestionId, default(Guid));

            ViewBag.Type = (int)MessageType.InviteAnswer;
            ViewBag.dataType = (int)MessageDataType.Question;
            ViewBag.dataId = QuestionId;
            ViewBag.content = question.QuestionContent.Length > 20 ? question.QuestionContent.Substring(0, 19) : question.QuestionContent;

            //当前已经成功邀请过的用户集合
            ViewBag.InviteSuccess = _userService.CheckTodayInviteSuccessTotal(userId, SchoolId, QuestionId, 4, 2, DateTime.Now);

            ViewBag.ShareDesc = $"{dto.AnswerCount}个回答\n" + SchoolName;
            string ShareLink = "/Question/QuestionDetail" + ShareUrlParameterJoin.ParameterJoin(HttpContext.Request.Query, new List<string>() { "SchoolId", "isSuccessQuestion" });
            //ViewBag.ShareLink = "/Question/QuestionDetail?QuestionId=" + QuestionId;
            ViewBag.ShareLink = ShareLink;

            return View();
        }

        /// <summary>
        /// 加载该学校下最新问题
        /// </summary>
        /// <param name="SchoolId"></param>
        /// <param name="PageIndex"></param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        [Description("加载该学校下最新问题")]
        public ResponseResult GetQuestion(Guid SchoolId, int PageIndex, int PageSize, bool isHot, QueryQuestion query = QueryQuestion.All, SelectedQuestionOrder Order = SelectedQuestionOrder.None, Guid SearchSchool = default(Guid))
        {
            try
            {
                //SearchSchool：筛选动态学校分部标签，获取指定学校分部数据，默认获取该学校所有分部数据

                //检测当前用户身份 （true：校方用户 |false：普通用户）
                ViewBag.isSchoolRole = false;
                Guid userId = Guid.Empty;
                if (User.Identity.IsAuthenticated)
                {
                    var user = User.Identity.GetUserInfo();
                    //检测当前用户身份 （true：校方用户 |false：普通用户）
                    ViewBag.isSchoolRole = user.Role.Contains((int)PMS.UserManage.Domain.Common.UserRole.School);
                    userId = user.UserId;
                }

                //学校分部type标签还原
                query = SearchSchool != default(Guid) ? (QueryQuestion)8 : query;

                if ((int)query == 8)
                {
                    SchoolId = SearchSchool;
                }

                List<QuestionVo> questionVos = new List<QuestionVo>();

                if (!isHot)
                {
                    questionVos.AddRange(_mapper.Map<List<QuestionDto>, List<QuestionVo>>(_questionInfo.GetNewQuestionInfoBySchoolId(SchoolId, userId, PageIndex, PageSize, query, Order, out int total)));
                }
                else
                {
                    questionVos.AddRange(_mapper.Map<List<QuestionDto>, List<QuestionVo>>(_questionInfo.GetHotQuestionInfoBySchoolId(SchoolId, userId, PageIndex, PageSize, SelectedQuestionOrder.Intelligence)));
                }

                var UserIds = new List<Guid>();

                UserIds.AddRange(questionVos.SelectMany(x => x.Answer.Select(u => u.UserId)).Distinct().ToList());
                var Users = _userService.ListUserInfo(UserIds.GroupBy(q => q).Select(p => p.Key).ToList());

                var extIds = questionVos.GroupBy(q => q.SchoolSectionId).Select(q => q.Key).ToList();
                var schoolInfos = _schoolInfoService.GetSchoolSectionQaByIds(extIds);

                questionVos.ForEach(x =>
                {
                    var schoolInfo = schoolInfos.FirstOrDefault(q => q.SchoolSectionId == x.SchoolSectionId);
                    x.School = _mapper.Map<SchoolInfoQaDto, QuestionVo.SchoolInfo>(schoolInfo);

                    if (x.Answer.Count > 0)
                    {
                        x.Answer[0].UserInfo = _mapper.Map<UserInfoVo>(Users.FirstOrDefault(q => q.Id == x.Answer[0].UserId)).ToAnonyUserName(x.Answer[0].IsAnony);
                    }

                    if (x.Images.Count() > 0)
                    {
                        for (int i = 0; i < x.Images.Count(); i++)
                        {
                            x.Images[i] = _setting.QueryImager + x.Images[i];
                        }
                    }
                    x.No = x.No.ToLower();
                });

                return ResponseResult.Success(new { rows = questionVos });
            }
            catch (Exception ex)
            {
                return ResponseResult.Failed(ex.Message);
            }
        }

        /// <summary>
        /// 获取该问题下的最新回答
        /// </summary>
        /// <param name="QuestionId"></param>
        /// <param name="PageIndex"></param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        [Description("获取问题最新回答")]
        public ResponseResult GetQuestionAnswer(Guid QuestionId, int PageIndex, int PageSize)
        {
            try
            {
                //检测当前用户身份 （true：校方用户 |false：普通用户）
                ViewBag.isSchoolRole = false;
                Guid userId = Guid.Empty;
                if (User.Identity.IsAuthenticated)
                {
                    var user = User.Identity.GetUserInfo();
                    //检测当前用户身份 （true：校方用户 |false：普通用户）
                    ViewBag.isSchoolRole = user.Role.Contains((int)PMS.UserManage.Domain.Common.UserRole.School);
                    userId = user.UserId;
                }

                var answerInfo = _questionsAnswers.GetNewAnswerInfoByQuestionId(QuestionId, userId, PageIndex, PageSize, out int total);

                var answerVo = _mapper.Map<List<AnswerInfoDto>, List<AnswerInfoVo>>(answerInfo.ToList());

                var UserIds = new List<Guid>();
                UserIds.AddRange(answerInfo.GroupBy(q => q.UserId).Select(p => p.Key).ToList());
                var Users = _userService.ListUserInfo(UserIds.GroupBy(q => q).Select(p => p.Key).ToList());

                if (answerVo.Count() > 0)
                {
                    var ReplyNewest = _questionsAnswers.ReplyNewest(answerVo.Select(x => x.Id).ToList());

                    //拿到用户信息
                    List<UserInfoVo> parentUsers = null;
                    if (ReplyNewest != null)
                    {
                        parentUsers = _mapper.Map<List<UserInfoDto>, List<UserInfoVo>>(_userService.ListUserInfo(ReplyNewest.Select(x => x.UserId).ToList()));
                    }

                    answerVo.ForEach(x =>
                    {
                        var answerUser = _mapper.Map<UserInfoVo>(Users.FirstOrDefault(q => q.Id == x.UserId));
                        x.UserInfo = answerUser.ToAnonyUserName(x.IsAnony);

                        var newest = ReplyNewest.Find(y => y.ParentId == x.Id);
                        if (newest != null)
                        {
                            var a = new ReplyDetailViewModel() { Id = newest.Id, UserInfo = parentUsers.Find(y => y.Id == newest.UserId).ToAnonyUserName(newest.IsAnony), Content = newest.AnswerContent, CreateTime = newest.AddTime };
                            x.NewestReply = a;
                        }

                    });
                }

                return ResponseResult.Success(new { rows = answerVo });
            }
            catch (Exception ex)
            {
                return ResponseResult.Failed(ex.Message);
            }
        }


        [HttpPost]
        [Authorize]
        [Sxb.Web.Authentication.Attribute.BindMobile]
        [Description("提交回答")]
        public ResponseResult QuestionReply([FromBody] AnswerAdd answerAdd)
        {
            return QuestionAnswer(answerAdd);
        }

        [HttpPost]
        [Authorize]
        [Sxb.Web.Authentication.Attribute.BindMobile]
        [Description("提交回答")]
        public ResponseResult QuestionAnswer(AnswerAdd answerAdd)
        {
            try
            {
                var user = User.Identity.GetUserInfo();
                string iSchoolAuth = HttpContext.Request.Cookies["iSchoolAuth"];

                if (string.IsNullOrWhiteSpace(answerAdd.Content))
                {
                    return ResponseResult.Failed("内容不能为空");
                }

                var contentCheck = _text.GarbageCheck(new ProductManagement.API.Aliyun.Model.GarbageCheckRequest()
                {
                    scenes = new[] { "antispam" },
                    tasks = new List<ProductManagement.API.Aliyun.Model.GarbageCheckRequest.Task>() {
                      new ProductManagement.API.Aliyun.Model.GarbageCheckRequest.Task(){
                       content=answerAdd.Content,
                         dataId = Guid.NewGuid().ToString()
                      }
                      }
                }).Result;

                if (contentCheck.data.FirstOrDefault().results.FirstOrDefault().suggestion == "block")
                {
                    return ResponseResult.Failed("您发布的内容包含敏感词，<br>请重新编辑后再发布。");
                }

                var answer = _mapper.Map<AnswerAdd, QuestionsAnswersInfo>(answerAdd);

                Guid answerUserID = Guid.Empty;
                string title = "";

                var question = _questionInfo.GetQuestionById(answer.QuestionInfoId, default(Guid));
                answerUserID = question.UserId;
                title = question.QuestionContent;

                answer.UserId = user.UserId;
                List<int> roles = new List<int>();

                var Job = _partTimeJobAdmin.GetModelById(user.UserId);

                if (Job != null)
                {
                    //为兼职用户
                    if (!_questionsAnswers.CheckAnswerDistinct(answerAdd.Content))
                    {
                        return ResponseResult.Failed("该回答内容不符合规则");
                    }

                    answer.PostUserRole = UserRole.JobMember;
                    roles = _adminRolereService.GetPartJobRoles(user.UserId).Select(x => x.Role).ToList();
                }
                else
                {
                    answer.PostUserRole = user.Role.Length > 0 ? (UserRole)user.Role[0] : UserRole.Member;
                }
                answer.CreateTime = DateTime.Now;
                answer.State = ExamineStatus.Unread;


                if (answer.ParentId != null)
                {
                    var FirstParent = _questionsAnswers.GetFirstParent((Guid)answer.ParentId);
                    answer.FirstParentId = FirstParent.Id;

                    var AnswerModel = _questionsAnswers.GetModelById((Guid)answer.ParentId);
                    answerUserID = AnswerModel.UserId;
                    title = AnswerModel.Content;
                }
                int rez = _questionsAnswers.Add(answer);

                //所有邀请我回答这个问题的消息, 全部设为已读,已处理
                _message.UpdateMessageHandled(true, MessageType.InviteAnswer, MessageDataType.Question, user.UserId, default, new List<Guid>() { answer.QuestionInfoId });

                if (rez > 0)
                {
                    //检测是否为自己回复自己 
                    if (answerUserID != user.UserId)
                    {
                        bool states = _inviteStatus.AddMessage(new PMS.UserManage.Domain.Entities.Message()
                        {
                            title = title,
                            Content = answerAdd.Content,
                            DataID = answer.Id,
                            DataType = (byte)MessageDataType.Answer,
                            EID = question.SchoolSectionId,
                            Type = (byte)MessageType.Reply,
                            userID = answerUserID,
                            senderID = user.UserId,
                            ReadChangeTime = DateTime.Now
                        });
                    }
                }
                //添加成功
                if (rez > 0)
                {
                    //if (answerAdd.ParentId == null)
                    //{
                    //    //修改问题回答总次数
                    //    _questionInfo.UpdateQuestionLikeorReplayCount(answer.QuestionInfoId, 1, false);
                    //    var detial = await _questionInfo.QuestionDetail(answer.QuestionInfoId, user.UserId);
                    //    rez = detial.AnswerCount;
                    //}
                    //else
                    //{
                    //    if ((Guid)answer.ParentId != (Guid)answer.FirstParentId)
                    //    {
                    //        _questionsAnswers.UpdateAnswerLikeorReplayCount((Guid)answer.ParentId, 1, false);
                    //        _questionsAnswers.UpdateAnswerLikeorReplayCount((Guid)answer.FirstParentId, 1, false);
                    //    }
                    //    rez = _questionsAnswers.GetModelById((Guid)answer.ParentId).ReplyCount;
                    //}

                    //检测是否为兼职领队用户，领队用户写点评，将自动新建兼职身份
                    if (roles.Contains(2) && !roles.Contains(1) && Job.SettlementType == SettlementType.SettlementWeChat)
                    {
                        //包含兼职领队身份，但还并未包含兼职身份时，写点评需要新增兼职用户身份，且直接归属到自身下的兼职

                        //新增兼职身份
                        _adminRolereService.Add(new PartTimeJobAdminRole() { AdminId = user.UserId, Role = 1, ParentId = user.UserId });

                        //新增任务周期
                        _amountMoneyService.NextSettlementData(Job, 1);
                    }
                }
                return ResponseResult.Success(new
                {
                    replyId = answer.Id,
                    successRow = rez
                });
            }
            catch (Exception ex)
            {
                return ResponseResult.Failed(ex.Message);
            }
        }

        [Description("邀请回答")]
        public IActionResult InviteAnswer()
        {
            return View();
        }

        [Description("推送学校问题")]
        public async Task<PageResult<List<QuestionVo>>> PushSchoolQuestion(Guid SchoolId)
        {
            try
            {
                Guid userId = Guid.Empty;
                if (User.Identity.IsAuthenticated)
                {
                    var user = User.Identity.GetUserInfo();
                    userId = user.UserId;
                }

                //接口调用，根据关键字模糊查询，得到学校集合id
                //List<Guid> schoolIds = new List<Guid>();
                //schoolIds.Add(Guid.Parse("B4D4A9BA-9D1E-4918-9876-540B61DE45EE"));
                //schoolIds.Add(Guid.Parse("D6FBC021-8833-4B4D-A4E0-8D8F5DFA8247"));
                //schoolIds.Add(Guid.Parse("28C8613A-7E7B-4CC5-B528-9027E65D0B67"));

                var extent = _schoolInfoService.GetSchoolName(new List<Guid>() { SchoolId }).FirstOrDefault();
                ////根据写点评的学校id，获取该学校的类型
                var school = await _schoolService.GetSchoolExtSimpleDtoAsync(0, 0, SchoolId);
                //如果学校分部为null的话返回空对象
                if (school == null)
                    return new PageResult<List<QuestionVo>>()
                    {
                        StatusCode = 200,
                        rows = null
                    };

                string key = $"IdenticalSchool:Province:{school.Province}&City:{school.City}&Area:{school.Area}&SchoolGrade:{extent.SchoolGrade}&SchoolType:{extent.SchoolType}";

                ////根据该类型得到该相同类型的学校，进行推送
                var result = await _easyRedisClient.GetOrAddAsync(key, async () =>
                {
                    return await _schoolService.GetSchoolExtListAsync(0, 0, school.Province, school.City, school.Area, new int[] { (int)extent.SchoolGrade }, 0, new int[] { (int)extent.SchoolType }, 0, new int[] { }, 1, 10); ;
                }, new TimeSpan(0, 30, 0));

                //获取相同类型学校
                //var result = await _schoolService.GetIdenticalSchool(SchoolId, 1, 10);

                //接口调用，根据关键字模糊查询，得到学校集合id
                List<Guid> schoolIds = new List<Guid>();
                schoolIds.AddRange(result.List.Select(x => x.ExtId));

                //排序（点赞+回答） 最高的前10问题
                var schools = _questionInfo.GetHotSchoolSectionId();

                List<QuestionVo> questions = new List<QuestionVo>();
                if (schoolIds.Count() > 0)
                {
                    key = $"QuestionIdenticalSchool:Province:{school.Province}&City:{school.City}&Area:{school.Area}&SchoolGrade:{extent.SchoolGrade}&SchoolType:{extent.SchoolType}";
                    var questionList = await _easyRedisClient.GetOrAddAsync(key, () =>
                    {
                        return _questionInfo.PushSchoolInfo(schoolIds, userId);
                    }, new TimeSpan(0, 30, 0));

                    //第一次根据学校id查询对应学校数据
                    questions.AddRange(_mapper.Map<List<QuestionDto>, List<QuestionVo>>(questionList));

                    //查询出无问答的数据个数
                    int noneQuestion = questions.Where(x => x.Id == default(Guid)).Count();

                    if (questions.Count() != 10 || noneQuestion != 0)
                    {
                        int HotSchoolTotal = schools.Count();

                        //检测是否存在点评
                        if (HotSchoolTotal > 0)
                        {
                            //第一次已经查询所有学校的问题
                            if (noneQuestion == 0)
                            {
                                questions.AddRange(_mapper.Map<List<QuestionDto>, List<QuestionVo>>(_questionInfo.PushSchoolInfo(schools.Take(10 - questions.Count())?.ToList(), userId)));
                            }
                            else
                            {
                                int takeTotal = 0;
                                if (noneQuestion == 10)
                                {
                                    takeTotal = 10;
                                }
                                else
                                {
                                    takeTotal = 10 - noneQuestion;
                                }
                                //第一次查询出的根据学校id的问题数据存在无问题情况，进行补全数据
                                questions.AddRange(_mapper.Map<List<QuestionDto>, List<QuestionVo>>(_questionInfo.PushSchoolInfo(schools.Take(takeTotal)?.ToList(), userId)));
                            }
                        }
                    }
                }
                else
                {
                    questions.AddRange(_mapper.Map<List<QuestionDto>, List<QuestionVo>>(_questionInfo.PushSchoolInfo(schools, userId)));
                }

                var extIds = questions.GroupBy(q => q.SchoolSectionId).Select(q => q.Key).ToList();
                var schoolInfos = _schoolInfoService.GetSchoolName(extIds);
                questions.ForEach(x =>
                {
                    var schoolInfo = schoolInfos.FirstOrDefault(q => q.SchoolSectionId == x.SchoolSectionId);
                    x.School = new QuestionVo.SchoolInfo()
                    {
                        SchoolName = schoolInfo.SchoolName
                    };
                });

                return new PageResult<List<QuestionVo>>()
                {
                    StatusCode = 200,
                    rows = questions.Where(x => x.Id != default(Guid))?.ToList()
                };
            }
            catch (Exception ex)
            {
                return new PageResult<List<QuestionVo>>()
                {
                    StatusCode = 500,
                    Message = ex.Message
                };
            }
        }

        public ResponseResult GetAnswerCardInfo(Guid answerId)
        {
            try
            {
                var answer = _questionsAnswers.GetAnswerInfoDtoByIds(new List<Guid>() { answerId }).FirstOrDefault();
                //学校详情信息
                var school = _schoolService.GetSchoolByIds(answer.SchoolSectionId.ToString()).FirstOrDefault();
               
                return ResponseResult.Success(new
                {
                    answer.Id,
                    answer.AnswerContent,
                    answer.questionDto.QuestionContent,
                    ShortQuestionNo = UrlShortIdUtil.Long2Base32(answer.questionDto.No),
                    school.FullSchoolName,
                    school.LodgingType,
                    school.ShortSchoolNo
                });
            }
            catch (Exception ex)
            {
                return ResponseResult.Failed(ex.Message);
            }
        }

        /// <summary>
        /// 一堆ID给你返回一个提问或者回答的list
        /// </summary>
        /// <param name="requestOptions"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public ResponseResult GetQuestionOrAnswer([FromBody] List<RequestOption> requestOptions)
        {
            try
            {

                if (User.Identity.IsAuthenticated)
                {
                    var userInfo = User.Identity.GetUserInfo();
                    QuestionAndAnswer questionAndAnswer = new QuestionAndAnswer();
                    var groups = requestOptions.GroupBy(x => x.dataIdType).ToList();

                    for (int i = 0; i < groups.Count(); i++)
                    {
                        if (groups[i].Key == dataIdType.Question)
                        {
                            List<Guid> questionIds = groups[i].Select(x => x.dataId).ToList();

                            var questions = _mapper.Map<List<QuestionDto>, List<QuestionVo>>(_questionInfo.GetQuestionByIds(questionIds, userInfo == null ? default(Guid) : userInfo.UserId));

                            List<QuestionVo> temp = new List<QuestionVo>();
                            foreach (var item in questionIds)
                            {
                                var questionTemp = questions.Where(x => x.Id == item).FirstOrDefault();
                                if (questionTemp != null)
                                {
                                    temp.Add(questionTemp);
                                }
                            }

                            questionAndAnswer.questionModels = temp;
                        }
                        else if (groups[i].Key == dataIdType.Answer)
                        {
                            questionAndAnswer.answerModels = _mapper.Map<List<AnswerInfoDto>, List<AnswerInfoVo>>(_questionsAnswers.GetAnswerInfoDtoByIds(groups[i].Select(x => x.dataId).ToList(), userInfo == null ? default(Guid) : userInfo.UserId));
                        }
                    }
                    return ResponseResult.Success(questionAndAnswer);
                }
                else
                {
                    return ResponseResult.Success();
                }
            }
            catch (Exception ex)
            {
                return ResponseResult.Failed(ex.Message);
            }
        }

        /// <summary>
        /// 当前用户发布的问题或者答案
        /// </summary>
        /// <param name="requestOptions"></param>
        /// <returns></returns>
        [Authorize]
        public ResponseResult GetNewestQuestionOrAnswer(int PageIndex = 1, int PageSize = 1)
        {
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    var user = User.Identity.GetUserInfo();

                    QuestionAndAnswer questionAndAnswer = new QuestionAndAnswer();

                    questionAndAnswer.answerModels = _mapper.Map<List<AnswerInfoDto>, List<AnswerInfoVo>>(_questionsAnswers.GetNewestAnswerInfoDtoByUserId(PageIndex, PageSize, user.UserId));

                    var result = _questionInfo.GetNewestQuestion(PageIndex, PageSize, user.UserId);
                    var SchoolSections = _schoolInfoService.GetSchoolStatuse(result.GroupBy(q => q.SchoolSectionId).Select(q => q.Key).ToList()).ToList();

                    questionAndAnswer.questionModels = _mapper.Map<List<QuestionDto>, List<QuestionVo>>(result);

                    questionAndAnswer.questionModels.ForEach(x =>
                    {
                        var schoolSection = SchoolSections.FirstOrDefault(q => q.SchoolSectionId == x.SchoolSectionId);
                        x.School = new QuestionVo.SchoolInfo()
                        {
                            SchoolName = schoolSection.SchoolName
                        };
                    });

                    return ResponseResult.Success(questionAndAnswer);
                }
                return ResponseResult.Success();
            }
            catch (Exception ex)
            {
                return ResponseResult.Failed(ex.Message);
            }
        }

        /// <summary>
        /// 当前用户发布的问题或者答案
        /// </summary>
        /// <param name="requestOptions"></param>
        /// <returns></returns>
        [Authorize]
        public ResponseResult GetCurrentUserNewestAnswer(int PageIndex = 1, int PageSize = 1)
        {
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    var userInfo = User.Identity.GetUserInfo();
                    QuestionAndAnswer questionAndAnswer = new QuestionAndAnswer();
                    var answerModels = _mapper.Map<List<AnswerInfoDto>, List<AnswerInfoVo>>(_questionsAnswers.GetCurrentUserNewestAnswer(PageIndex, PageSize, userInfo.UserId));
                    return ResponseResult.Success(answerModels);
                }
                return ResponseResult.Success();
            }
            catch (Exception ex)
            {
                return ResponseResult.Failed(ex.Message);
            }
        }

        /// <summary>
        /// 获取当前用户点赞的回答已经回答回复
        /// </summary>
        /// <param name="PageIndex"></param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        [Authorize]
        public ResponseResult GetLikeAnswerAndAnswerReply(int PageIndex = 1, int PageSize = 10)
        {
            List<LikeQuestionAndAnswerModel> commentReplyAndReplyModel = new List<LikeQuestionAndAnswerModel>();
            //用户id
            List<Guid> userIds = new List<Guid>();

            var userInfo = User.Identity.GetUserInfo();

            //userInfo.UserId = Guid.Parse("ec2bdc19-d148-4afa-87ed-40749bb81f31");
            //获取问题回复的
            var commentAndAnswer = _questionsAnswers.CurrentLikeQuestionAndAnswer(userInfo.UserId, PageIndex, PageSize);

            //获取问题列表
            var question = _questionInfo.GetQuestionByIds(commentAndAnswer.GroupBy(x => x.QuestionInfoId).Select(x => x.Key)?.ToList(), userInfo.UserId)?.ToList();
            List<Guid> ExtIds = question.GroupBy(x => x.SchoolSectionId).Select(x => x.Key)?.ToList();

            var school = _schoolInfoService.GetSchoolStatuse(ExtIds);

            var reply = commentAndAnswer.Where(x => x.ParentId != default(Guid))?.ToList();
            List<AnswerInfoDto> answer = new List<AnswerInfoDto>();
            if (reply != null)
            {
                var answe = _questionsAnswers.GetAnswerInfoDtoByIds(reply.Select(x => x.ParentId).ToList(), userInfo.UserId)?.ToList();
                answer.AddRange(answe);
            }

            if (answer.Count() > 0)
            {
                userIds.AddRange(answer.Select(x => x.UserId));
            }
            userIds.AddRange(commentAndAnswer.GroupBy(x => x.UserId).Select(x => x.Key)?.ToList());

            //用户
            var Users = _userService.ListUserInfo(userIds.GroupBy(q => q).Select(p => p.Key).ToList());

            foreach (var item in commentAndAnswer)
            {
                LikeQuestionAndAnswerModel questionAnswer = new LikeQuestionAndAnswerModel();
                //问题
                var q = question.Where(x => x.Id == item.QuestionInfoId).FirstOrDefault();
                if (item.Type == 0)
                {
                    questionAnswer.Content = q.QuestionContent;
                    var answerUser = Users.Where(x => x.Id == item.UserId).FirstOrDefault();
                    questionAnswer.answerUserInfoVo = UserHelper.ToAnonyUserName(new UserInfoVo()
                    {
                        Id = answerUser.Id,
                        HeadImager = answerUser.HeadImgUrl,
                        NickName = answerUser.NickName,
                        Role = answerUser.VerifyTypes.ToList()
                    }, item.IsAnony);
                    questionAnswer.AnswerContent = item.Content;
                    questionAnswer.Id = q.Id;
                    questionAnswer.ParentAnswerId = item.Id;
                    questionAnswer.AddTime = q.QuestionCreateTime.ConciseTime();
                    questionAnswer.AnswerAddTime = item.CreateTime.ConciseTime();
                }
                else
                {
                    //回复
                    var answerInfo = answer.Where(x => x.Id == item.ParentId).FirstOrDefault();
                    //回复者
                    var answerUserInfo = Users.Where(x => x.Id == answerInfo.UserId).FirstOrDefault();

                    //问题发布者信息
                    questionAnswer.userInfoVo = UserHelper.ToAnonyUserName(new UserInfoVo()
                    {
                        Id = answerUserInfo.Id,
                        NickName = answerUserInfo.NickName,
                        HeadImager = answerUserInfo.HeadImgUrl,
                        Role = answerUserInfo.VerifyTypes.ToList()
                    }, answerInfo.IsAnony);

                    var user = Users.Where(x => x.Id == item.UserId).FirstOrDefault();

                    questionAnswer.answerUserInfoVo = UserHelper.ToAnonyUserName(new UserInfoVo()
                    {
                        Id = user.Id,
                        HeadImager = user.HeadImgUrl,
                        NickName = user.NickName,
                        Role = user.VerifyTypes.ToList()
                    }, item.IsAnony);

                    questionAnswer.Content = item.Content;
                    questionAnswer.Id = item.Id;
                    questionAnswer.ParentAnswerId = answerInfo.Id;
                    questionAnswer.AnswerContent = answerInfo.AnswerContent;
                    questionAnswer.AddTime = item.CreateTime.ConciseTime();
                    questionAnswer.AnswerAddTime = answerInfo.AddTime;
                }
                questionAnswer.SchoolId = q.SchoolId;
                questionAnswer.SchoolSectionId = q.SchoolSectionId;
                questionAnswer.SchoolName = school.Where(x => x.SchoolSectionId == q.SchoolSectionId).FirstOrDefault()?.SchoolName;
                questionAnswer.LikeCount = item.LikeCount;
                questionAnswer.AnswerCount = item.ReplyCount;
                questionAnswer.Type = item.Type;
                commentReplyAndReplyModel.Add(questionAnswer);
            }
            return ResponseResult.Success(commentReplyAndReplyModel);
        }

        /// <summary>
        /// 当前用户发布的问题
        /// </summary>
        /// <param name="PageIndex"></param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        [Authorize]
        public ResponseResult CurrentPublishQuestion(int PageIndex = 1, int PageSize = 10)
        {
            try
            {
                var userInfo = User.Identity.GetUserInfo();
                var result = _questionInfo.GetNewestQuestion(PageIndex, PageSize, userInfo.UserId);
                var SchoolSections = _schoolInfoService.GetSchoolStatuse(result.GroupBy(q => q.SchoolSectionId).Select(q => q.Key).ToList()).ToList();

                var question = _mapper.Map<List<QuestionDto>, List<QuestionVo>>(result);

                question.ForEach(x =>
                {
                    var schoolSection = SchoolSections.FirstOrDefault(q => q.SchoolSectionId == x.SchoolSectionId);
                    x.School = new QuestionVo.SchoolInfo()
                    {
                        SchoolName = schoolSection.SchoolName
                    };
                });

                return ResponseResult.Success(question);
            }
            catch (Exception ex)
            {
                return ResponseResult.Failed(ex.Message);
            }
        }

        /// <summary>
        /// 当前用户发布的“回答”+当前用户发布的“回答”的回复
        /// </summary>
        /// <param name="PageIndex"></param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        [Authorize]
        public ResponseResult CurrentPublishQuestionAnswerAndReply(int PageIndex = 1, int PageSize = 10)
        {
            List<QuestionAnswerAndAnswerReplyModel> commentReplyAndReplyModel = new List<QuestionAnswerAndAnswerReplyModel>();
            //用户id
            List<Guid> userIds = new List<Guid>();

            var userInfo = User.Identity.GetUserInfo();
            //获取问题回复的
            var commentAndAnswer = _questionsAnswers.CurrentPublishQuestionAnswerAndReply(userInfo.UserId, PageIndex, PageSize);

            //获取问题列表
            var question = _questionInfo.GetQuestionByIds(commentAndAnswer.GroupBy(x => x.QuestionInfoId).Select(x => x.Key)?.ToList(), userInfo.UserId)?.ToList();
            if (question != null)
            {
                userIds.AddRange(question.Select(x => x.UserId)?.ToList());
            }
            List<Guid> ExtIds = question.GroupBy(x => x.SchoolSectionId).Select(x => x.Key)?.ToList();

            //学校
            var school = _schoolInfoService.GetSchoolStatuse(ExtIds);

            var reply = commentAndAnswer.Where(x => x.ParentId != default(Guid))?.ToList();
            List<AnswerInfoDto> answer = new List<AnswerInfoDto>();
            if (reply != null)
            {
                var answe = _questionsAnswers.GetAnswerInfoDtoByIds(reply.Select(x => x.ParentId).ToList(), userInfo.UserId)?.ToList();
                answer.AddRange(answe);
            }

            if (answer.Count() > 0)
            {
                userIds.AddRange(answer.Select(x => x.UserId));
            }
            userIds.AddRange(commentAndAnswer.GroupBy(x => x.UserId).Select(x => x.Key)?.ToList());

            //用户
            var Users = _userService.ListUserInfo(userIds.GroupBy(q => q).Select(p => p.Key).ToList());

            foreach (var item in commentAndAnswer)
            {
                QuestionAnswerAndAnswerReplyModel questionAnswer = new QuestionAnswerAndAnswerReplyModel();
                //问题
                var q = question.Where(x => x.Id == item.QuestionInfoId).FirstOrDefault();
                if (item.Type == 0)
                {
                    //发表问题用户
                    var u = Users.Where(x => x.Id == q.UserId).FirstOrDefault();
                    //问题发布者信息
                    questionAnswer.userInfoVo = UserHelper.ToAnonyUserName(new UserInfoVo()
                    {
                        Id = u.Id,
                        NickName = u.NickName,
                        HeadImager = u.HeadImgUrl,
                        Role = u.VerifyTypes.ToList()
                    }, q.IsAnony);
                    questionAnswer.Content = q.QuestionContent;
                    var answerUser = Users.Where(x => x.Id == item.UserId).FirstOrDefault();
                    questionAnswer.answerUserInfoVo = UserHelper.ToAnonyUserName(new UserInfoVo()
                    {
                        Id = answerUser.Id,
                        HeadImager = answerUser.HeadImgUrl,
                        NickName = answerUser.NickName,
                        Role = answerUser.VerifyTypes.ToList()
                    }, item.IsAnony);
                    questionAnswer.AnswerContent = item.Content;
                    questionAnswer.Id = q.Id;
                    questionAnswer.AnswerId = item.Id;
                    questionAnswer.AddTime = q.QuestionCreateTime.ConciseTime();
                    questionAnswer.AnswerAddTime = item.CreateTime.ConciseTime();
                }
                else
                {
                    //发表问题用户
                    var u = Users.Where(x => x.Id == q.UserId).FirstOrDefault();
                    //回复
                    var answerInfo = answer.Where(x => x.Id == item.ParentId).FirstOrDefault();
                    //回复者
                    var answerUserInfo = Users.Where(x => x.Id == answerInfo.UserId).FirstOrDefault();

                    //问题发布者信息
                    questionAnswer.userInfoVo = UserHelper.ToAnonyUserName(new UserInfoVo()
                    {
                        Id = u.Id,
                        NickName = u.NickName,
                        HeadImager = u.HeadImgUrl,
                        Role = u.VerifyTypes.ToList()
                    }, q.IsAnony);

                    questionAnswer.answerUserInfoVo = UserHelper.ToAnonyUserName(new UserInfoVo()
                    {
                        Id = answerUserInfo.Id,
                        HeadImager = answerUserInfo.HeadImgUrl,
                        NickName = answerUserInfo.NickName,
                        Role = answerUserInfo.VerifyTypes.ToList()
                    }, answerInfo.IsAnony);

                    questionAnswer.Content = item.Content;
                    questionAnswer.Id = item.Id;
                    questionAnswer.AnswerId = answerInfo.Id;
                    questionAnswer.AnswerContent = answerInfo.AnswerContent;
                    questionAnswer.AddTime = item.CreateTime.ConciseTime();
                    questionAnswer.AnswerAddTime = answerInfo.AddTime;
                }
                questionAnswer.SchoolId = q.SchoolId;
                questionAnswer.SchoolSectionId = q.SchoolSectionId;
                questionAnswer.SchoolName = school.Where(x => x.SchoolSectionId == q.SchoolSectionId).FirstOrDefault()?.SchoolName;
                questionAnswer.LikeCount = item.LikeCount;
                questionAnswer.AnswerCount = item.ReplyCount;
                questionAnswer.Type = item.Type;
                commentReplyAndReplyModel.Add(questionAnswer);
            }
            return ResponseResult.Success(commentReplyAndReplyModel);
        }

        [Authorize]
        public void Tes()
        {
            try
            {

                if (User.Identity.IsAuthenticated)
                {
                    List<RequestOption> requestOptions = new List<RequestOption>()
                    {
                        new RequestOption(){dataIdType = dataIdType.Question,dataId=Guid.Parse("f6ca2f44-4725-42bc-a11f-9aa8ae8604fd")},
                        new RequestOption(){dataIdType = dataIdType.Question,dataId=Guid.Parse("f775676c-91cd-4006-ad2d-aab02eb4046f")},
                        new RequestOption(){dataIdType = dataIdType.Question,dataId=Guid.Parse("19bacc1e-31c5-4446-bcc1-8200f9c9c3c1")},
                        new RequestOption(){dataIdType = dataIdType.Question,dataId=Guid.Parse("85652f2d-e40b-47f4-b083-b5b7ae4bd4e2")},
                        new RequestOption(){dataIdType = dataIdType.Question,dataId=Guid.Parse("08acc082-bb97-4809-ade4-d79b052eecc3")},
                        new RequestOption(){dataIdType = dataIdType.Question,dataId=Guid.Parse("320bd9fa-f6f3-4e8a-94eb-cb7b32049bc7")},
                        new RequestOption(){dataIdType = dataIdType.Question,dataId=Guid.Parse("f97eb733-37ec-4a81-9db5-a2d376698dde")},
                        new RequestOption(){dataIdType = dataIdType.Question,dataId=Guid.Parse("7b1241c1-18c8-45fd-ba2d-97737d45508d")},
                        new RequestOption(){dataIdType = dataIdType.Question,dataId=Guid.Parse("547b6b72-ff12-4186-b0f2-3e2886c9f7ff")},
                        new RequestOption(){dataIdType = dataIdType.Question,dataId=Guid.Parse("cc2f6b29-cda4-4db4-aaa4-925c2d0b1a82")}
                    };

                    var userInfo = User.Identity.GetUserInfo();
                    QuestionAndAnswer questionAndAnswer = new QuestionAndAnswer();
                    var groups = requestOptions.GroupBy(x => x.dataIdType).ToList();

                    for (int i = 0; i < groups.Count(); i++)
                    {
                        if (groups[i].Key == dataIdType.Question)
                        {
                            var questions = _mapper.Map<List<QuestionDto>, List<QuestionVo>>(_questionInfo.GetQuestionByIds(groups[i].Select(x => x.dataId).ToList(), userInfo == null ? default(Guid) : userInfo.UserId));

                            questionAndAnswer.questionModels = questions;
                        }
                        else if (groups[i].Key == dataIdType.Answer)
                        {
                            questionAndAnswer.answerModels = _mapper.Map<List<AnswerInfoDto>, List<AnswerInfoVo>>(_questionsAnswers.GetAnswerInfoDtoByIds(groups[i].Select(x => x.dataId).ToList(), userInfo == null ? default(Guid) : userInfo.UserId));
                        }
                    }
                    //return ResponseResult.Success(questionAndAnswer);
                }
                else
                {
                    //return ResponseResult.Success();
                }
            }
            catch (Exception)
            {
                //return ResponseResult.Failed(ex.Message);
            }
        }

        /// <summary>
        /// 点评回复页
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [Description("点评回复页")]
        public IActionResult Reply(Guid Id)
        {
            Guid userId = Guid.Empty;
            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity.GetUserInfo();
                userId = user.UserId;
            }


            ViewBag.DatascoureId = Id;
            ViewBag.Type = 2;
            var isLike = _likeService.CheckCurrentUserIsLike(userId, Id);


            //检测当前用户身份 （true：校方用户 |false：普通用户）
            ViewBag.isSchoolRole = false;
            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity.GetUserInfo();
                //检测当前用户身份 （true：校方用户 |false：普通用户）
                ViewBag.isSchoolRole = user.Role.Contains((int)UserRole.School);
            }
            ViewBag.Type = 2;

            var answerId = Id;
            var answer = _questionsAnswers.QueryAnswerInfo(answerId);

            if (answer == null)
            {
                throw new NotFoundException();
            }
            _questionsAnswers.UpdateAnswerViewCount(answerId);

            var hottestAnswerReply = _questionsAnswers.GetAnswerHottestReplys(answerId);

            List<Guid> userIds = hottestAnswerReply.Select(x => x.UserId)?.ToList();
            userIds.Add(answer.UserId);

            var users = _userService.ListUserInfo(userIds);

            var replyUser = users.FirstOrDefault(q => q.Id == answer.UserId);


            var reply = new ReplyDetailViewModel
            {
                Id = answer.Id,
                DatascoureId = answer.QuestionId,
                SchoolSectionId = answer.SchoolSectionId,
                Content = answer.AnswerContent,
                IsLike = isLike,
                IsAttend = answer.IsAttend,
                IsSchoolPublish = answer.IsSchoolPublish,
                RumorRefuting = answer.RumorRefuting,
                ReplyCount = answer.ReplyCount,
                LikeCount = answer.LikeCount,
                CreateTime = answer.AddTime,
                UserInfo = answer.IsAnony ?
                new UserInfoVo
                {
                    NickName = "匿名用户",
                    HeadImager = "https://cdn.sxkid.com//images/AppComment/defaultUserHeadImage.png"
                } :
                new UserInfoVo
                {
                    Id = answer.UserId,
                    NickName = replyUser.NickName,
                    HeadImager = replyUser.HeadImgUrl,
                    Role = replyUser.VerifyTypes.ToList()
                }.ToAnonyUserName(answer.IsAnony)
            };


            List<Guid> replyIds = hottestAnswerReply.GroupBy(q => q.Id).Select(s => s.Key).ToList();
            var isLikes = _likeService.CheckCurrentUserIsLikeBulk(userId, replyIds);
            //var hottestAnswerReplyViewModel = hottestAnswerReply.Select(q => new ReplyViewModel
            //{
            //    Id = q.Id,
            //    Content = q.AnswerContent,
            //    DatascoureId = q.QuestionId,
            //    ReplyId = q.ParentId,
            //    IsLike = isLikes.FirstOrDefault(p => p.SourceId == q.Id) != null && isLikes.FirstOrDefault(p => p.SourceId == q.Id).IsLike,
            //    IsTop = false,
            //    IsAttend = q.IsAttend,
            //    IsSchoolPublish = q.IsSchoolPublish,
            //    RumorRefuting = q.RumorRefuting,
            //    ReplyCount = q.ReplyCount,
            //    LikeCount = q.LikeCount,
            //    UserInfo = _mapper.Map<UserInfoVo>(users.FirstOrDefault(p => p.Id == q.UserId)).ToAnonyUserName(q.IsAnony),
            //    CreateTime = q.AddTime
            //}).ToList();

            //ViewBag.replyHottest = hottestAnswerReplyViewModel;

            ViewBag.replyDetail = reply;

            return View();
        }


    }
}