using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PMS.CommentsManage.Application.IServices;
using PMS.CommentsManage.Application.ModelDto;
using PMS.CommentsManage.Domain.Common;
using PMS.CommentsManage.Domain.Entities;
using PMS.Infrastructure.Application.IService;
using PMS.School.Application.IServices;
using PMS.Search.Application.IServices;
using PMS.Search.Application.ModelDto.Query;
using PMS.UserManage.Application.IServices;
using PMS.UserManage.Domain.Common;
using ProductManagement.API.Aliyun;
using ProductManagement.API.Http.Interface;
using ProductManagement.Framework.Cache.Redis;
using ProductManagement.Framework.Foundation;
using ProductManagement.Infrastructure.Exceptions;
using Sxb.PCWeb.Common;
using Sxb.PCWeb.Models;
using Sxb.PCWeb.Models.User;
using Sxb.PCWeb.RequestModel.Question;
using Sxb.PCWeb.Response;
using Sxb.PCWeb.Utils;
using Sxb.PCWeb.Utils.CommentHelper;
using Sxb.PCWeb.ViewModels.Comment;
using Sxb.PCWeb.ViewModels.Question;
using Sxb.PCWeb.ViewModels.School;
using Sxb.Web.RequestModel.Question;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using static PMS.UserManage.Domain.Common.EnumSet;

namespace Sxb.PCWeb.Controllers
{
    public class QuestionController : BaseController
    {
        private readonly IMessageService _inviteStatus;

        private readonly ImageSetting _setting;
        private readonly IQuestionInfoService _questionInfo;
        private readonly ISchoolService _schoolService;
        private readonly ISchoolInfoService _schoolInfoService;
        private readonly IQuestionsAnswersInfoService _answerService;
        private readonly IGiveLikeService _likeService;
        private readonly IUserService _userService;
        private readonly IEasyRedisClient _easyRedisClient;
        readonly ICityInfoService _cityService;
        ISchService _SchService;

        private readonly ISchoolImageService _imageService;
        private readonly IUserServiceClient _userMessage;
        //管理员
        private readonly IPartTimeJobAdminService _partTimeJobAdmin;

        private readonly IHistoryService _historyService;
        private readonly ISearchService _dataSearch;
        private readonly IImportService _dataImport;
        private IPartTimeJobAdminRolereService _adminRolereService;
        private ISettlementAmountMoneyService _amountMoneyService;
        private readonly ISchoolCommentService _commentService;

        private readonly ICollectionService _collectionService;


        //热门点评、学校组合方法
        PullHottestCSHelper hot;

        private readonly IMapper _mapper;

        private readonly IText _text;
        public QuestionController(IEasyRedisClient easyRedisClient, ICityInfoService cityCodeService, IOptions<ImageSetting> set,
            IQuestionsAnswersInfoService questionsAnswers, IUserService userService,
            ISchoolService schoolService, IQuestionInfoService questionInfo,
            IUserServiceClient userServiceClient, ISchoolInfoService schoolInfoService,
            IPartTimeJobAdminService partTimeJobAdmin, IGiveLikeService likeService,
        IUserServiceClient userMessage, ISearchService dataSearch, IImportService dataImport,
            ISchoolCommentService commentService, ICollectionService collectionService,
            ISchoolImageService imageService, IHistoryService historyService,
            IPartTimeJobAdminRolereService adminRolereService, ISettlementAmountMoneyService amountMoneyService,
            IMapper mapper, IText text, IMessageService inviteStatus, ISchService schService)
        {
            _imageService = imageService;
            _easyRedisClient = easyRedisClient;
            _cityService = cityCodeService;
            _schoolInfoService = schoolInfoService;
            _userMessage = userMessage;
            _answerService = questionsAnswers;
            _setting = set.Value;
            _questionInfo = questionInfo;
            _schoolService = schoolService;
            _userService = userService;
            _commentService = commentService;
            _partTimeJobAdmin = partTimeJobAdmin;
            _likeService = likeService;
            _collectionService = collectionService;
            _SchService = schService;

            _historyService = historyService;
            _inviteStatus = inviteStatus;
            _adminRolereService = adminRolereService;
            _amountMoneyService = amountMoneyService;
            _text = text;

            hot = new PullHottestCSHelper(likeService, _userService, schoolInfoService, _setting, schService);

            _dataSearch = dataSearch;
            _dataImport = dataImport;
            _mapper = mapper;
        }



        /// <summary>
        /// 热门问答列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("{controller}/{action}")]
        public async Task<ResponseResult> GetHotQuestions()
        {
            var data = hot.HotQuestion(await _questionInfo.GetHotQuestion());
            return ResponseResult.Success(data);
        }

        /// <summary>
        /// 提问/学校视图列表视图页
        /// </summary>
        /// <returns></returns>
        [Description("问答列表页")]
        [Route("{controller}/{action=List}")]
        public async Task<IActionResult> List(string search, string type = "question", int page = 1, int city = 0)
        {
            int pageSize = 10;
            ViewBag.PageIndex = page;
            ViewBag.PageSize = pageSize;

            ViewBag.KeyWords = search;
            ViewBag.ListType = type;
            //获取当前的城市
            var localCityCode = city == 0 ? Request.GetLocalCity() : city;
            ViewBag.LocalCity = await _cityService.GetCityName(localCityCode);
            //获取热搜词
            ViewBag.HotSearchWord = await _easyRedisClient.GetOrAddAsync("HotSearchQuestionWord", () =>
            {
                return _dataSearch.GetHotHistoryList(2, 6, null, localCityCode);
            }, new TimeSpan(0, 10, 0));
            //热门城市
            var hotCity = await _cityService.GetHotCity();
            ViewBag.HotCity = hotCity;

            //用户id
            Guid UserId = Guid.Empty;
            if (User.Identity.IsAuthenticated)
            {
                UserId = User.Identity.GetId();
            }
            string UUID = Request.GetDevice();
            //获取学校提问
            //var QuestionDtos = _questionInfo.QuestionList_Pc(UserId, localCityCode, 1, 10);

            ////得到所有提问的分部学校id
            //var schoolExtIds = QuestionDtos?.Select(x => x.SchoolSectionId).ToList();

            ////学校列表实体
            //var schoolExts = _schoolInfoService.GetSchoolSectionByIds(schoolExtIds);

            //需要获取沈总学校分数
            //var score = ScoreToStarHelper.CommentScoreToStar(QuestionDtos.Select(x => x.Score)?.ToList());
            //var question = QuestionDtoToVoHelper.QuestionDtoToVo(_setting.QueryImager, QuestionDtos, UserHelper.UserDtoToVo(Users), SchoolDtoToSchoolCardHelper.SchoolDtoToSchoolQuestionCard(schoolExts));


            if (type == "question")
            {

                //问答视图实体
                List<QuestionInfoViewModel> questions = new List<QuestionInfoViewModel>();
                //问答dto
                List<QuestionDto> questionDtos = new List<QuestionDto>();
                string UA = Request.Headers["User-Agent"].ToString();
                if (!string.IsNullOrWhiteSpace(search) && !UA.ToLower().Contains("spider") && !UA.ToLower().Contains("bot") && Request.GetAvaliableCity() != 0)
                {
                    if (User.Identity.IsAuthenticated)
                    {
                        var user = User.Identity.GetUserInfo();
                        _dataImport.SetHistory("PC", 2, search, Request.GetAvaliableCity(), user.UserId, UUID);
                    }
                    else
                    {
                        _dataImport.SetHistory("PC", 2, search, Request.GetAvaliableCity(), null, UUID);
                    }
                }
                var result = _dataSearch.SearchQuestion(new SearchQuestionQuery
                {
                    Keyword = search,
                    Orderby = string.IsNullOrWhiteSpace(search) ? 1 : 0,//没有搜索词默认已发布时间倒序
                    PageNo = page,
                    PageSize = pageSize,
                    CityIds = new List<int> { localCityCode }
                });
                //检测是否有数据返回
                if (result.Total > 0)
                {
                    questionDtos = _questionInfo.GetQuestionByIds(result.Questions.Select(q => q.Id).ToList(), UserId);
                    questionDtos = result.Questions.Where(q => questionDtos.Any(p => p.Id == q.Id))
                        .Select(q => questionDtos.FirstOrDefault(p => p.Id == q.Id)).ToList();
                }
                ViewBag.TotalPage = result.Total;
                //}
                //else
                //{
                //    string CommentKey = $"commentList:city_{localCityCode}:pageindex_{page}:pagesize_{pageSize}";

                //    questionDtos = _questionInfo.QuestionList_Pc(UserId, localCityCode, page, pageSize, out int Total);
                //    ViewBag.TotalPage = Total;
                //}
                var UserIds = new List<Guid>();
                questionDtos.Where(q => q.answer != null).ToList().ForEach(p => { UserIds.AddRange(p.answer.Select(q => q.UserId)); });
                UserIds.AddRange(questionDtos.GroupBy(q => q.UserId).Select(p => p.Key).ToList());
                var Users = _userService.ListUserInfo(UserIds.GroupBy(q => q).Select(p => p.Key).ToList());

                var schoolIds = questionDtos.GroupBy(q => q.SchoolId).Select(q => q.Key).ToList();
                var extIds = questionDtos.GroupBy(q => q.SchoolSectionId).Select(q => q.Key).ToList();

                //学校列表实体
                var schoolExts = _schoolInfoService.GetSchoolSectionByIds(extIds);

                //检测提问是否关注
                var checkCollection = _collectionService.GetCollection(questionDtos.Select(x => x.Id).ToList(), UserId);

                questions = QuestionDtoToVoHelper.QuestionDtoToVo(_setting.QueryImager, questionDtos, UserHelper.UserDtoToVo(Users),
                schoolExts.Select(q => new SchoolQuestionCardViewModel
                {
                    ExtId = q.SchoolSectionId,
                    Sid = q.SchoolId,
                    SchoolName = q.SchoolName,
                    Auth = q.IsAuth,
                    International = q.SchoolType == PMS.School.Domain.Common.SchoolType.International,
                    LodgingType = (int)q.LodgingType,
                    LodgingReason = q.LodgingType.Description(),
                    Type = (int)q.SchoolType,
                    QuestionTotal = q.SectionQuestionTotal,
                    ShortSchoolNo = q.ShortSchoolNo
                }).ToList(), checkCollection);
                //if (data.Any())
                //{
                //    string schoolKey = $"SchoolInfos:{ DesTool.Md5(string.Join("_", extIds))}";
                //    string schoolQaTotalKey = $"SchoolQaTotalInfos:{ DesTool.Md5(string.Join("_", schoolIds))}";

                //    var schoolInfos = await _easyRedisClient.GetOrAddAsync(schoolKey, () =>
                //    {
                //        return _schoolInfoService.GetSchoolSectionQaByIds(extIds);
                //    }, new TimeSpan(0, 5, 0));

                //    var schoolQuestionTotal = await _easyRedisClient.GetOrAddAsync(schoolQaTotalKey, () =>
                //    {
                //        return _questionInfo.SchoolTotalQuestion(schoolIds);
                //    }, new TimeSpan(0, 5, 0));
                //}
                ViewBag.QuestionList = questions;
            }
            else
            {
                var schoolQueryData = new SearchSchoolQuery();
                if (string.IsNullOrWhiteSpace(search))
                {
                    schoolQueryData = new SearchSchoolQuery
                    {
                        CurrentCity = localCityCode,
                        PageNo = page,
                        PageSize = pageSize,
                        Orderby = 5
                    };
                }
                else
                {
                    if (User.Identity.IsAuthenticated)
                    {
                        var user = User.Identity.GetUserInfo();
                        _dataImport.SetHistory("PC", 3, search, localCityCode, user.UserId, UUID);
                    }
                    else
                    {
                        _dataImport.SetHistory("PC", 3, search, localCityCode, null, UUID);
                    }

                    schoolQueryData = new SearchSchoolQuery
                    {
                        Keyword = search,
                        PageNo = page,
                        PageSize = pageSize,
                    };
                }

                var list = _dataSearch.SearchSchool(schoolQueryData);

                var ids = list.Schools.Select(q => q.Id).ToList();

                var result = ids.Count > 0 ? _schoolInfoService.GetSchoolSectionByIds(ids) : new List<SchoolInfoDto>();

                //学校列表实体

                var data = list.Schools.Where(q => result.Any(p => p.SchoolSectionId == q.Id)).Select(q =>
                {
                    var r = result.FirstOrDefault(p => p.SchoolSectionId == q.Id);
                    return r == null ? new QuestionSchoolViewModel() : new QuestionSchoolViewModel
                    {
                        ExtId = r.SchoolSectionId,
                        //Url = SchoolDetailUrl(r.ExtId, r.Sid),
                        SchoolName = r.SchoolName,
                        LodgingType = (int)r.LodgingType,
                        LodgingReason = r.LodgingType.Description(),
                        Type = (int)r.SchoolType,
                        International = r.SchoolType == PMS.School.Domain.Common.SchoolType.International,
                        Auth = r.IsAuth,
                        QuestionTotal = r.SectionQuestionTotal,
                        Score = r.SchoolAvgScore,
                        ShortSchoolNo = r.ShortSchoolNo
                    };
                });
                ViewBag.TotalPage = list.Total;
                ViewBag.SchoolList = data?.ToList();
            }

            //热问
            ViewBag.HottestSchoolQuesitonView = hot.HotQuestion(await _questionInfo.GetHotQuestion());

            //热问学校【侧边栏】
            //热问学校
            ViewBag.HottestSchoolView = hot.HottestQuestionItem(await _questionInfo.HottestSchool());

            return View();
        }

        /// <summary>
        /// 学校问答列表页
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [Description("学校问答列表页")]
        [Route("/{controller}/{action}/{Id}")]
        [Route("/{controller}/school-{schoolNo}")]
        [Route("/{controller}/school-{schoolNo}/{tab}")]
        public async Task<IActionResult> School(Guid Id, string search, int page = 1, int Tab = 1, string schoolNo = default)
        {
            Guid ExtId = Id;
            if (!string.IsNullOrWhiteSpace(schoolNo))
            {
                var ext = await _SchService.GetSchextSimpleInfoViaShortNo(schoolNo);
                if (ext != null)
                {
                    ExtId = ext.Eid;
                }
                else
                {
                    return new ExtNotFoundViewResult();
                }
            }
            //获取该校统计数据
            var SchoolInfo = await _schoolInfoService.QuerySchoolInfo(ExtId);

            var schoolData = await _schoolService.GetSchoolExtDtoAsync(ExtId, 0, 0);

            int SelectedTab = Tab;
            int pageSize = 10;
            ViewBag.PageIndex = page;
            ViewBag.PageSize = pageSize;
            ViewBag.ExtId = ExtId;
            ViewBag.KeyWords = search;
            //检测当前用户身份 （true：校方用户 |false：普通用户）
            ViewBag.isSchoolRole = false;

            Guid UserId = Guid.Empty;
            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity.GetUserInfo();
                //检测当前用户身份 （true：校方用户 |false：普通用户）
                ViewBag.isSchoolRole = user.Role.Contains((int)PMS.UserManage.Domain.Common.UserRole.School);
                UserId = user.UserId;

                //检测改用户是否关注该学校
                //var checkCollectedSchool = await _userServiceClient.IsCollected(new ProductManagement.API.Http.Model.IsCollected() { userId = userId, dataID = SchoolId });
                //ViewBag.isCollected = checkCollectedSchool.iscollected;
            }

            ViewBag.SelectedTab = SelectedTab;


            //获取学校提问列表页 标签统计值及标签 【现在统计数据都为统计全校（所有分部）】
            var QuestionTotal = _questionInfo.CurrentQuestionTotalBySchoolId(SchoolInfo.SchoolId)
            .Select(q => new SchoolQuestionTotalViewModel
            {
                SchoolSectionId = q.SchoolSectionId,
                Name = q.Name,
                Total = q.Total,
                TotalType = q.TotalType.Description(),
                TotalTypeNumber = (int)q.TotalType
            }).ToList();

            //获取该校下所有的分部名称标签
            var AllExtension = _schoolService.GetSchoolExtName(SchoolInfo.SchoolId);
            for (int i = 0; i < AllExtension.Count(); i++)
            {
                var item = QuestionTotal.FirstOrDefault(x => x.SchoolSectionId == AllExtension[i].Value);
                int currentIndex = QuestionTotal.IndexOf(item);
                if (item == null)
                {
                    QuestionTotal.Add(new SchoolQuestionTotalViewModel() { Name = AllExtension[i].Key, SchoolSectionId = AllExtension[i].Value, TotalType = QueryCondition.Other.Description(), TotalTypeNumber = (int)QueryCondition.Other });
                }
                else
                {
                    item.Name = AllExtension[i].Key;
                    QuestionTotal[currentIndex] = item;
                }
            }

            //将其他分部类型学校进行再次排序
            int TotalTypeNumber = 8;
            QuestionTotal.Where(x => x.TotalTypeNumber == 8)?.ToList().ForEach(x =>
            {
                x.TotalTypeNumber = TotalTypeNumber;
                TotalTypeNumber++;
            });

            //领域实体转 视图实体【学校问答卡片】
            var SchoolQuestionCard = SchoolDtoToSchoolCardHelper.SchoolDtoToSchoolQuestionCard(new List<SchoolInfoDto>() { SchoolInfo });

            ViewBag.SchoolQuestionCard = SchoolQuestionCard.FirstOrDefault();
            ViewBag.QuestionTotal = QuestionTotal;


            if (string.IsNullOrWhiteSpace(search))
            {
                List<QuestionDto> HotQuestion = new List<QuestionDto>();
                if (SelectedTab == 1)
                {
                    //获取学校热门问题数据
                    HotQuestion = _questionInfo.GetHotQuestionInfoBySchoolId(SchoolInfo.SchoolSectionId, UserId, 1, 3, SelectedQuestionOrder.Intelligence) ?? new List<QuestionDto>();
                }

                List<QuestionDto> NewQuestion;
                int total = 0;

                if (SelectedTab >= 8)
                {
                    Guid eid = QuestionTotal.Where(x => x.TotalTypeNumber == SelectedTab).FirstOrDefault().SchoolSectionId;
                    //获取该校的最新问题
                    NewQuestion = _questionInfo.GetNewQuestionInfoBySchoolId(eid, UserId, page, 10, QueryQuestion.Other, SelectedQuestionOrder.CreateTime, out total) ?? new List<QuestionDto>();
                }
                else
                {
                    //获取该校的最新问题
                    NewQuestion = _questionInfo.GetNewQuestionInfoBySchoolId(SchoolInfo.SchoolId, UserId, page, 10, (QueryQuestion)SelectedTab, SelectedQuestionOrder.CreateTime, out total) ?? new List<QuestionDto>();
                }

                //获取热门提问写入用户信息
                var userIds = HotQuestion.Select(p => p.UserId).ToList();
                userIds.AddRange(NewQuestion.Select(p => p.UserId).ToList());
                var Users = _userService.ListUserInfo(userIds);

                //检测提问是否关注
                var checkCollection = _collectionService.GetCollection(HotQuestion.Select(x => x.Id).ToList(), UserId);
                checkCollection.AddRange(_collectionService.GetCollection(NewQuestion.Select(x => x.Id).ToList(), UserId));

                //提问领域实体转视图实体
                var HotQuestionViewModels = QuestionDtoToVoHelper.QuestionDtoToVo(_setting.QueryImager, HotQuestion, UserHelper.UserDtoToVo(Users), null, checkCollection);
                ViewBag.HotQuestionList = HotQuestionViewModels;

                var NewQuestionViewModels = QuestionDtoToVoHelper.QuestionDtoToVo(_setting.QueryImager, NewQuestion, UserHelper.UserDtoToVo(Users), null, checkCollection);
                ViewBag.NewQuestionList = NewQuestionViewModels;
                ViewBag.TotalPage = total;

            }
            else
            {
                var result = _dataSearch.SearchQuestion(new SearchQuestionQuery
                {
                    Keyword = search,
                    PageNo = page,
                    PageSize = pageSize,
                    Eid = ExtId
                });
                List<QuestionDto> SearchQuestion = new List<QuestionDto>();
                //检测是否有数据返回
                if (result.Total > 0)
                {
                    SearchQuestion = _questionInfo.GetQuestionByIds(result.Questions.Select(x => x.Id).ToList(), UserId);
                }
                ViewBag.TotalPage = result.Total;
                //获取热门点评写入用户信息
                var UserIds = new List<Guid>();
                UserIds.AddRange(SearchQuestion.GroupBy(q => q.UserId).Select(p => p.Key).ToList());
                var Users = _userService.ListUserInfo(UserIds.GroupBy(q => q).Select(p => p.Key).ToList());

                var NewQuestionViewModels = QuestionDtoToVoHelper.QuestionDtoToVo(_setting.QueryImager, SearchQuestion, UserHelper.UserDtoToVo(Users));
                ViewBag.NewQuestionList = NewQuestionViewModels;
            }
            //热问
            ViewBag.HottestSchoolQuesitonView = hot.HotQuestion(await _questionInfo.GetHotQuestion());

            //热问学校【侧边栏】
            //热问学校
            ViewBag.HottestSchoolView = hot.HottestQuestionItem(await _questionInfo.HottestSchool());

            // 热评
            //先调用沈总接口获取用户学校推荐算法值【如果为游客用户，可以则直接推荐该城市的全类型数据】
            //热门点评列表【侧边栏】
            var date_Now = DateTime.Now;
            ViewBag.HottestComment = hot.HotComment(await _commentService.HotComment(new HotCommentQuery()
            {
                Condition = true,
                City = Request.GetLocalCity(),
                Grade = 1,
                Type = 1,
                Discount = false,
                Diglossia = false,
                Chinese = false,
                StartTime = date_Now.AddMonths(-6),
                EndTime = date_Now
            }, UserId), UserId);

            //热评学校【侧边栏】
            //热评学校
            ViewBag.HottestCommentSchoolView = hot.HottestSchool(await _commentService.HottestSchool(new HotCommentQuery()
            {
                Condition = true,
                City = Request.GetLocalCity(),
                Grade = 1,
                Type = 1,
                Discount = false,
                Diglossia = false,
                Chinese = false,
                StartTime = date_Now.AddMonths(-6),
                EndTime = date_Now
            }, false));

            ViewBag.Page_Description = schoolData?.Intro?.GetHtmlHeaderString(150) ?? string.Empty;

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
                ViewBag.IsSchoolRole = false;
                Guid UserId = Guid.Empty;
                if (User.Identity.IsAuthenticated)
                {
                    var user = User.Identity.GetUserInfo();
                    //检测当前用户身份 （true：校方用户 |false：普通用户）
                    ViewBag.IsSchoolRole = user.Role.Contains((int)PMS.UserManage.Domain.Common.UserRole.School);
                    UserId = user.UserId;
                }

                //学校分部type标签还原
                query = SearchSchool != default(Guid) ? (QueryQuestion)8 : query;

                if ((int)query == 8)
                {
                    SchoolId = SearchSchool;
                }

                List<QuestionDto> QuestionDtos = new List<QuestionDto>();
                if (!isHot)
                {
                    QuestionDtos = _questionInfo.GetNewQuestionInfoBySchoolId(SchoolId, UserId, PageIndex, PageSize, query, Order, out int total);
                }
                else
                {
                    QuestionDtos = _questionInfo.GetHotQuestionInfoBySchoolId(SchoolId, UserId, PageIndex, PageSize, SelectedQuestionOrder.Intelligence);
                }

                //获取最新提问下的用户列表实体
                var Users = _userService.ListUserInfo(QuestionDtos.Select(x => x.UserId).ToList());

                //检测提问是否关注
                var checkCollection = _collectionService.GetCollection(QuestionDtos.Select(x => x.Id).ToList(), UserId);

                //提问领域层转 视图层实体
                var QuestionViewModels = QuestionDtoToVoHelper.QuestionDtoToVo(_setting.QueryImager, QuestionDtos, UserHelper.UserDtoToVo(Users), null, checkCollection);

                return ResponseResult.Success(new { rows = QuestionViewModels });
            }
            catch (Exception ex)
            {
                return ResponseResult.Failed(ex.Message);
            }
        }

        /// <summary>
        /// 问答详情
        /// </summary>
        /// <param name="QuestionId"></param>
        /// <returns></returns>
        [Description("问答详情")]
        [Route("question/{No}.html")]
        [Route("question/detail/{QuestionId}")]
        public async Task<IActionResult> Detail(Guid QuestionId, string No, int page = 1)
        {
            int pageSize = 10;
            ViewBag.PageIndex = page;
            ViewBag.PageSize = pageSize;


            //检测当前用户身份 （true：校方用户 |false：普通用户）
            ViewBag.IsSchoolRole = false;
            //检测是否关注该问题
            ViewBag.IsCollected = false;
            Guid UserId = Guid.Empty;

            //问题详情
            QuestionDto QuestionDetail = new QuestionDto();

            long numberId = 0;

            if (No != null)
            {
                numberId = UrlShortIdUtil.Base322Long(No);
            }

            if (numberId > 0)
            {
                QuestionDetail = await _questionInfo.QuestionDetailByNo(numberId, UserId);
            }
            else
            {
                QuestionDetail = _questionInfo.QuestionDetail(QuestionId, UserId);
            }

            if (QuestionDetail == null)
            {

            }

            QuestionId = QuestionDetail.Id;

            //检测当前用户相关操作信息
            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity.GetUserInfo();
                //检测当前用户身份 （true：校方用户 |false：普通用户）
                ViewBag.IsSchoolRole = user.Role.Contains((int)PMS.UserManage.Domain.Common.UserRole.School);
                UserId = user.UserId;

                //检测改用户是否关注该问题
                var isCollected = _collectionService.IsCollected(UserId, QuestionId, CollectionDataType.QA);
                ViewBag.IsCollected = isCollected;
                //添加用户访问历史记录
                _historyService.AddHistory(UserId, QuestionDetail.Id, (byte)MessageDataType.Question);
            }

            ViewBag.No = UrlShortIdUtil.Long2Base32(QuestionDetail.No);
            _questionInfo.UpdateQuestionViewCount(QuestionDetail.Id);


            ViewBag.Id = QuestionDetail.Id;

            //热门回答
            var HotAnswers = _answerService.GetAnswerInfoByQuestionId(QuestionDetail.Id, UserId, 1, 3);

            //最新回答
            var NewAnswers = _answerService.GetNewAnswerInfoByQuestionId(QuestionDetail.Id, UserId, page, pageSize, out int total);

            //学校统计信息【学校卡片信息】
            var SchoolTotal = await _schoolInfoService.QuerySchoolInfo(QuestionDetail.SchoolSectionId);

            if (SchoolTotal == null)
            {
                throw new NotFoundException();
            }

            //获取【问题 | 回答 | 当前用户】 信息实体
            var UserIds = new List<Guid>();
            UserIds.AddRange(HotAnswers.GroupBy(q => q.UserId).Select(p => p.Key).ToList());
            UserIds.AddRange(NewAnswers.GroupBy(q => q.UserId).Select(p => p.Key).ToList());
            UserIds.Add(QuestionDetail.UserId);
            UserIds.Add(UserId);
            var Users = _userService.ListUserInfo(UserIds.GroupBy(q => q).Select(p => p.Key).ToList());

            //视图层用户实体列表
            var UserVos = UserHelper.UserDtoToVo(Users);


            //学校卡片信息
            var SchoolQuestionCard = SchoolDtoToSchoolCardHelper.SchoolDtoToSchoolQuestionCard(new List<SchoolInfoDto>() { SchoolTotal });


            //检测提问是否关注
            var checkCollection = _collectionService.GetCollection(new List<Guid>() { QuestionDetail.Id }, UserId);

            //提问信息领域层实体转 视图层
            ViewBag.QuestionDetail = QuestionDtoToVoHelper.QuestionDtoToVo(_setting.QueryImager, new List<QuestionDto>() { QuestionDetail }, UserVos, SchoolQuestionCard, checkCollection).FirstOrDefault();

            //热门回答领域层实体转 视图层实体
            ViewBag.HotAnswers = AnswerInfoDtoToVoHelper.AnswerInfoDtoToVo(HotAnswers, UserVos);

            //最新回答领域层实体转 视图层实体
            ViewBag.NewAnswers = AnswerInfoDtoToVoHelper.AnswerInfoDtoToVo(NewAnswers, UserVos);
            ViewBag.TotalPage = total;//todo:需要查询问题总回复数
                                      //热问
            ViewBag.HottestSchoolQuesitonView = hot.HotQuestion(await _questionInfo.GetHotQuestion());

            //热问学校【侧边栏】
            //热问学校
            ViewBag.HottestSchoolView = hot.HottestQuestionItem(await _questionInfo.HottestSchool());

            return View();
        }

        /// <summary>
        /// 获取该问题下的最新回答
        /// </summary>
        /// <param name="QuestionId"></param>
        /// <param name="PageIndex"></param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        [Description("获取问题最新回答")]
        public ResponseResult GetNewAnswers(Guid QuestionId, int PageIndex, int PageSize)
        {
            try
            {
                //检测当前用户身份 （true：校方用户 |false：普通用户）
                ViewBag.IsSchoolRole = false;
                Guid UserId = Guid.Empty;
                if (User.Identity.IsAuthenticated)
                {
                    var user = User.Identity.GetUserInfo();
                    //检测当前用户身份 （true：校方用户 |false：普通用户）
                    ViewBag.IsSchoolRole = user.Role.Contains((int)PMS.UserManage.Domain.Common.UserRole.School);
                    UserId = user.UserId;
                }

                //获取问题下最新回复信息
                var AnswerInfo = _answerService.GetNewAnswerInfoByQuestionId(QuestionId, UserId, PageIndex, PageSize, out int total);

                //获取回复写入用户信息实体
                var Users = _userService.ListUserInfo(AnswerInfo.Select(x => x.UserId)?.ToList()).ToList();

                return ResponseResult.Success(new { rows = AnswerInfoDtoToVoHelper.AnswerInfoDtoToVo(AnswerInfo, UserHelper.UserDtoToVo(Users)) });
            }
            catch (Exception ex)
            {
                return ResponseResult.Failed(ex.Message);
            }
        }

        /// <summary>
        /// 问答回复页
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [Description("问答回复页")]
        public async Task<IActionResult> Reply(Guid Id, int page = 1)
        {
            int pageSize = 10;
            ViewBag.PageIndex = page;
            ViewBag.PageSize = pageSize;
            ViewBag.Id = Id;

            Guid userId = Guid.Empty;
            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity.GetUserInfo();
                userId = user.UserId;
            }

            var answer = _answerService.QueryAnswerInfo(Id);

            if (answer == null)
            {
                throw new NotFoundException();
            }

            var extSchInfo = await _SchService.GetSchextSimpleInfo(answer.SchoolSectionId);

            var answerId = Id;
            _answerService.UpdateAnswerViewCount(answerId);

            var answerReplies = _answerService.PageAnswerReply(answerId, userId, 1, page, 10);

            int answerRepliesTotal = _answerService.GetAnswerReplyTotal(answerId);

            List<Guid> replyIds = answerReplies.GroupBy(q => q.Id).Select(s => s.Key).ToList();

            List<LikeDto> isLikes = new List<LikeDto>();
            if (replyIds.Count > 0)
            {
                isLikes = _likeService.CheckCurrentUserIsLikeBulk(userId, replyIds);
            }

            List<Guid> userIds = new List<Guid> { answer.UserId };
            userIds.AddRange(answerReplies.GroupBy(q => q.UserId).Select(s => (Guid)s.Key).ToList());
            var users = _userService.ListUserInfo(userIds);
            var replyUser = users.FirstOrDefault(q => q.Id == answer.UserId);


            List<Guid> parentUserIds = answerReplies.Where(p => p.ParentId != null).GroupBy(q => q.ParentUserId).Select(s => (Guid)s.Key).ToList();
            var parentUsers = _userService.ListUserInfo(parentUserIds);


            var reply = new QuestionAnswerViewModel
            {
                Id = answer.Id,
                QuestionId = answer.QuestionId,
                Content = answer.AnswerContent,
                IsLike = answer.IsLike,
                IsAttend = answer.IsAttend,
                IsSchoolPublish = answer.IsSchoolPublish,
                RumorRefuting = answer.RumorRefuting,
                AnswerTotal = answer.ReplyCount,
                LikeTotal = answer.LikeCount,
                AddTime = answer.AddTime,
                UserInfoVo = new UserInfoVo
                {
                    Id = answer.UserId,
                    NickName = replyUser.NickName,
                    HeadImager = replyUser.HeadImgUrl,
                    Role = replyUser.VerifyTypes.ToList()
                }.ToAnonyUserName(answer.IsAnony),
                SchoolExtName = extSchInfo?.ExtName,
                SchoolName = extSchInfo?.SchName,
                ShortSchoolNo = extSchInfo?.ShortSchoolNo,
            };
            ViewBag.ReplyDetail = reply;


            ViewBag.ReplyList = answerReplies.Select(q => new QuestionAnswerViewModel
            {
                Id = q.Id,
                Content = q.AnswerContent,
                IsLike = isLikes.FirstOrDefault(p => p.SourceId == q.Id) != null && isLikes.FirstOrDefault(p => p.SourceId == q.Id).IsLike,
                IsAttend = q.IsAttend,
                IsSchoolPublish = q.IsSchoolPublish,
                RumorRefuting = q.RumorRefuting,
                AnswerTotal = q.ReplyCount,
                LikeTotal = q.LikeCount,
                UserInfoVo = UserHelper.UserDtoToVo(users).Where(x => x.Id == q.UserId).FirstOrDefault(),
                AddTime = q.AddTime
            }).ToList();
            ViewBag.TotalPage = answerRepliesTotal;

            //热问
            ViewBag.HottestSchoolQuesitonView = hot.HotQuestion(await _questionInfo.GetHotQuestion());

            //热问学校【侧边栏】
            //热问学校
            ViewBag.HottestSchoolView = hot.HottestQuestionItem(await _questionInfo.HottestSchool());

            return View();
        }
        [HttpPost]
        [Authorize]
        [Description("提交回答")]
        public async Task<ResponseResult> QuestionReply(AnswerAdd answerAdd)
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


                answer.UserId = user.UserId;
                List<int> roles = new List<int>();

                if (!_partTimeJobAdmin.isExists(x => x.Id == userId))
                {
                    //为兼职用户

                    if (!_answerService.CheckAnswerDistinct(answerAdd.Content))
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
                    var FirstParent = _answerService.GetFirstParent((Guid)answer.ParentId);
                    answer.FirstParentId = FirstParent.Id;
                }
                int rez = _answerService.Insert(answer);

                var question = _questionInfo.GetQuestionById(answer.QuestionInfoId, default(Guid));

                if (rez > 0)
                {
                    //所有邀请我回复这个提问的消息, 设为已读,已处理
                    _inviteStatus.UpdateMessageHandled(true, MessageType.InviteAnswer, MessageDataType.Question, user.UserId, default, new List<Guid>() { answer.QuestionInfoId });

                    //检测是否为自己回复自己 
                    if (question.UserId != user.UserId)
                    {
                        bool states = _inviteStatus.AddMessage(new PMS.UserManage.Domain.Entities.Message()
                        {
                            Content = answerAdd.Content,
                            DataID = answer.Id,
                            DataType = (byte)MessageDataType.Question,
                            EID = question.SchoolSectionId,
                            Type = (byte)MessageType.Reply,
                            userID = question.UserId,
                            senderID = user.UserId
                        });
                    }
                }
                //添加成功
                if (rez > 0)
                {
                    if (answerAdd.ParentId == null)
                    {
                        //修改问题回答总次数
                        _questionInfo.UpdateQuestionLikeorReplayCount(answer.QuestionInfoId, 1, false);
                        var detial = _questionInfo.QuestionDetail(answer.QuestionInfoId, user.UserId);
                        rez = detial.AnswerCount;
                    }
                    else
                    {
                        if ((Guid)answer.ParentId != (Guid)answer.FirstParentId)
                        {
                            _answerService.UpdateAnswerLikeorReplayCount((Guid)answer.ParentId, 1, false);
                            _answerService.UpdateAnswerLikeorReplayCount((Guid)answer.FirstParentId, 1, false);
                        }
                        rez = _answerService.GetModelById((Guid)answer.ParentId).ReplyCount;
                    }

                    var Job = _partTimeJobAdmin.GetModelById(user.UserId);

                    //检测是否为兼职领队用户，领队用户写点评，将自动新建兼职身份
                    if (roles.Contains(2) && !roles.Contains(1) && Job.SettlementType == SettlementType.SettlementWeChat)
                    {
                        //包含兼职领队身份，但还并未包含兼职身份时，写点评需要新增兼职用户身份，且直接归属到自身下的兼职

                        //新增兼职身份
                        _adminRolereService.Add(new PartTimeJobAdminRole() { AdminId = user.UserId, Role = 1, ParentId = user.UserId });

                        //新增任务周期
                        _amountMoneyService.NextSettlementData(new PartTimeJobAdmin() { Id = user.UserId }, 1);
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

        /// <summary>
        /// 提问页
        /// </summary>
        /// <param name="eid"></param>
        /// <returns></returns>
        [Authorize]
        [Description("提问页")]
        public async Task<IActionResult> Write(Guid eid)
        {
            var schoolInfo = _schoolInfoService.GetSchoolName(new List<Guid>() { eid });

            bool isSchool = false;

            if (User.Identity.GetUserInfo().Role.Contains((int)PMS.UserManage.Domain.Common.UserRole.School))
            {
                isSchool = true;
            }

            ViewBag.IsSchool = isSchool;
            var schoolViewModel = SchoolDtoToSchoolCardHelper.SchoolDtoToSchoolCommentCard(schoolInfo).FirstOrDefault();
            ViewBag.SchoolInfo = schoolViewModel;

            var allSchoolExt = _schoolInfoService.GetCurrentSchoolAllExt(schoolViewModel.Sid);
            var currentSchool = allSchoolExt.Where(x => x.SchoolSectionId == eid).FirstOrDefault();
            ViewBag.AllSchoolBranch = allSchoolExt.Where(x => x.SchoolSectionId != eid);

            //热问
            ViewBag.HottestSchoolQuesitonView = hot.HotQuestion(await _questionInfo.GetHotQuestion());

            //热问学校【侧边栏】
            //热问学校
            ViewBag.HottestSchoolView = hot.HottestQuestionItem(await _questionInfo.HottestSchool());

            return View();
        }

        /// <summary>
        /// 提问提交
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [Description("提交问题")]
        public ResponseResult CommitQuestion(QuestionWriterViewModel question, Guid questionId, List<string> questionImages)
        {
            try
            {
                var ext = _schoolService.GetSchoolExtension(question.SchoolSectionId);
                if (ext == null)
                {
                    return ResponseResult.Failed("该学部不存在");
                }
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

                var user = User.Identity.GetUserInfo();
                question.Id = questionId;

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
                var QuestionInfo = _mapper.Map<QuestionWriterViewModel, QuestionInfo>(question);
                QuestionInfo.PostUserRole = user.Role.Length > 0 ? (UserRole)user.Role[0] : UserRole.Member;
                _questionInfo.AddQuestion(QuestionInfo);

                //所有邀请我提问这个学校的消息, 全部设为已读,已处理
                _inviteStatus.UpdateMessageHandled(true, MessageType.InviteQuestion, MessageDataType.School, user.UserId, default, new List<Guid>() { QuestionInfo.SchoolSectionId });


                if (imgs.Count != 0)
                {
                    foreach (var img in imgs)
                    {
                        _imageService.AddSchoolImage(img);
                    }
                }

                return ResponseResult.Success(new
                {
                    Eid = question.SchoolSectionId,
                    No = ext.ShortSchoolNo
                });
            }
            catch (Exception ex)
            {
                return ResponseResult.Failed(ex.Message);
            }
        }

        /// <summary>
        /// 提交问题回答
        /// </summary>
        /// <param name="answerAdd"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Description("提交问题回答")]
        public async Task<ResponseResult> PostAnswer(AnswerAdd answerAdd)
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

                answer.UserId = user.UserId;

                if (_partTimeJobAdmin.GetModelById(user.UserId) != null)
                {
                    answer.PostUserRole = UserRole.JobMember;
                }
                else
                {
                    answer.PostUserRole = user.Role.Length > 0 ? (UserRole)user.Role[0] : UserRole.Member;
                }
                answer.CreateTime = DateTime.Now;
                answer.State = ExamineStatus.Unread;


                if (answer.ParentId != null)
                {
                    var FirstParent = _answerService.GetFirstParent((Guid)answer.ParentId);
                    answer.FirstParentId = FirstParent.Id;
                }
                int rez = _answerService.Insert(answer);

                var question = _questionInfo.GetQuestionById(answer.QuestionInfoId, default(Guid));
                answerUserID = question.UserId;
                title = question.QuestionContent;
                if (rez > 0)
                {
                    //检测是否为自己回复自己 
                    if (answerUserID != user.UserId)
                    {
                        bool states = _inviteStatus.AddMessage(new PMS.UserManage.Domain.Entities.Message()
                        {
                            Content = answerAdd.Content,
                            DataID = answer.Id,
                            DataType = (byte)MessageDataType.Question,
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
                    if (answerAdd.ParentId == null)
                    {
                        //修改问题回答总次数
                        _questionInfo.UpdateQuestionLikeorReplayCount(answer.QuestionInfoId, 1, false);
                        var detial = _questionInfo.QuestionDetail(answer.QuestionInfoId, user.UserId);
                        rez = detial.AnswerCount;
                    }
                    else
                    {
                        if ((Guid)answer.ParentId != (Guid)answer.FirstParentId)
                        {
                            _answerService.UpdateAnswerLikeorReplayCount((Guid)answer.ParentId, 1, false);
                            _answerService.UpdateAnswerLikeorReplayCount((Guid)answer.FirstParentId, 1, false);
                        }

                        var Parent = _answerService.GetModelById((Guid)answer.ParentId);
                        rez = Parent.ReplyCount;

                        answerUserID = Parent.UserId;
                        title = Parent.Content;
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


        [HttpPost]
        [Authorize]
        [Description("提交回答")]
        public async Task<ResponseResult> Answer([FromBody] AnswerAdd answerAdd)
        {
            return await QuestionAnswer(answerAdd);
        }

        [HttpPost]
        [Authorize]
        [Description("提交回答")]
        public async Task<ResponseResult> QuestionAnswer(AnswerAdd answerAdd)
        {
            try
            {
                var user = User.Identity.GetUserInfo();
                string iSchoolAuth = HttpContext.Request.Cookies["iSchoolAuth"];

                if (string.IsNullOrWhiteSpace(answerAdd.Content))
                {
                    return ResponseResult.Failed("内容不能为空");
                }
                var question = _questionInfo.GetQuestionById(answerAdd.QuestionInfoId, default(Guid));
                if (question == null)
                {
                    return ResponseResult.Failed("回答的问题不存在");
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

                answer.UserId = user.UserId;
                List<int> roles = new List<int>();
                var Job = _partTimeJobAdmin.GetModelById(user.UserId);
                if (Job != null)
                {
                    //为兼职用户

                    if (!_answerService.CheckAnswerDistinct(answerAdd.Content))
                    {
                        answer.State = ExamineStatus.Block;
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
                    var FirstParent = _answerService.GetFirstParent((Guid)answer.ParentId);
                    answer.FirstParentId = FirstParent.Id;

                    var AnswerModel = _answerService.GetModelById((Guid)answer.ParentId);
                    answerUserID = AnswerModel.UserId;
                    title = AnswerModel.Content;
                }
                int rez = _answerService.Insert(answer);

                answerUserID = question.UserId;
                title = question.QuestionContent;


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
                            DataType = (byte)MessageDataType.Question,
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
                    //        _answerService.UpdateAnswerLikeorReplayCount((Guid)answer.ParentId, 1, false);
                    //        _answerService.UpdateAnswerLikeorReplayCount((Guid)answer.FirstParentId, 1, false);
                    //    }
                    //    rez = _answerService.GetModelById((Guid)answer.ParentId).ReplyCount;
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
    }
}