using iSchool;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PMS.CommentsManage.Application.IServices;
using PMS.Infrastructure.Application.IService;
using PMS.OperationPlateform.Application.IServices;
using PMS.OperationPlateform.Domain.Entitys;
using PMS.OperationPlateform.Domain.Enums;
using PMS.School.Application.IServices;
using PMS.School.Domain.Dtos;
using PMS.Search.Application.IServices;
using PMS.Search.Application.ModelDto.Query;
using PMS.UserManage.Application.IServices;
using ProductManagement.API.Aliyun;
using ProductManagement.Framework.Cache.Redis;
using ProductManagement.Framework.Foundation;
using Sxb.PCWeb.Common;
using Sxb.PCWeb.Models;
using Sxb.PCWeb.Response;
using Sxb.PCWeb.Utils;
using Sxb.PCWeb.Utils.CommentHelper;
using Sxb.PCWeb.ViewModels.Article;
using Sxb.PCWeb.ViewModels.Rank;
using Sxb.PCWeb.ViewModels.School;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using static PMS.UserManage.Domain.Common.EnumSet;

namespace Sxb.PCWeb.Controllers
{
    /// <summary>
    /// 文章H5模块
    /// </summary>
    ///
    public class ArticleController : BaseController
    {
        protected IArticleService articleService;
        protected IArticleCoverService articleCoverService;
        protected ISearchService _dataSearch;
        private readonly IHistoryService _historyService;
        protected IQuestionInfoService questionInfoService;
        protected IUserService _userService;
        protected ISchoolInfoService _schoolInfoService;
        protected IQuestionsAnswersInfoService questionsAnswersInfoService;
        protected ICityInfoService _cityCodeService;
        protected ISchoolService schoolService;
        protected ISchService _schService;
        protected IImportService _dataImport;
        private readonly ImageSetting _setting;
        private ICollectionService _collectionService;
        private readonly IText _text;
        private readonly IEasyRedisClient _easyRedisClient;

        private readonly ISchoolCommentService _commentService;
        private readonly ISchoolRankService _schoolRankService;
        IGeneralTagService _generalTagService;

        //热门点评、学校组合方法
        private PullHottestCSHelper hot;

        public ArticleController(
            IGeneralTagService generalTagService,
             IArticleService articleService,
             IArticleCoverService articleCoverService,
             ISearchService _dataSearch,
            IQuestionInfoService questionInfoService,
              IUserService _userService,
              ISchoolInfoService _schoolInfoService,
              IQuestionsAnswersInfoService questionsAnswersInfoService,
               ISchoolService schoolService,
               ICityInfoService _cityCodeService,
               ISchService _schService,
               IEasyRedisClient _easyRedisClient,
               IImportService _dataImport,
               IGiveLikeService likeservice,
               IOptions<ImageSetting> set,
               ISchoolCommentService commentService,
               ICollectionService collectionService,
               IEasyRedisClient easyRedisClient,
               IText text, IHistoryService historyService,
               ISchoolRankService schoolRankService
            )
        {
            _generalTagService = generalTagService;
            this.articleService = articleService;
            this.articleCoverService = articleCoverService;
            this._dataSearch = _dataSearch;
            this.questionInfoService = questionInfoService;
            this._userService = _userService;
            this._schoolInfoService = _schoolInfoService;
            this.questionsAnswersInfoService = questionsAnswersInfoService;
            this.schoolService = schoolService;
            this._cityCodeService = _cityCodeService;
            this._schService = _schService;
            this._dataImport = _dataImport;
            this._commentService = commentService;
            _historyService = historyService;
            _text = text;
            _easyRedisClient = easyRedisClient;

            this._setting = set.Value;
            this._collectionService = collectionService;
            this.hot = new PullHottestCSHelper(likeservice, _userService, _schoolInfoService, set.Value, _schService);
            this._schoolRankService = schoolRankService;
        }

        public Guid? UserId { get { return null; } }

        /// <summary>
        /// 文章Tag分页列表
        /// </summary>
        /// <param name="city">城市</param>
        /// <param name="search">搜索</param>
        /// <param name="offset">偏移量</param>
        /// <param name="limit">页大小</param>
        /// <param name="page">页码</param>
        /// <modify author="qzy" time="2020-07-13 14:58:49">增加page参数支持</modify>
        /// <returns></returns>
        [HttpGet]
        [Route("/articletag/{tagName}/{page}.html")]
        [Route("/articletag/{tagName}.html")]
        [Route("{controller}/{action=List}")]
        [Route("{controller}/{cityCode}-{page}")]
        [Route("{controller}/{page}")]
        [Route("{controller}")]
        [Description("文章列表")]
        public async Task<IActionResult> List(int city, string search, int offset = 0, int limit = 20, int page = 0, int cityCode = 0, string tagName = null)
        {
            offset = offset < 0 ? 0 : offset;

            if (page > 0)
            {
                offset = (page - 1) * limit;
            }

            int locationCityCode = city == 0 ? Request.GetLocalCity() : city;
            if (locationCityCode != cityCode && string.IsNullOrWhiteSpace(tagName))
            {
                if (string.IsNullOrWhiteSpace(search))
                {
                    return RedirectPermanent($"/article/{locationCityCode}-1/");
                }
                else
                {
                    return RedirectPermanent($"/article/{locationCityCode}-1?search=" + search);
                }
            }
            string UUID = Request.GetDevice();
            var latitude = Convert.ToDouble(Request.GetLatitude());
            var longitude = Convert.ToDouble(Request.GetLongitude());
            IEnumerable<article> articles = null;   //输出的文章列表
            int total = 0;  //输出的条数
            if (!string.IsNullOrWhiteSpace(tagName))//便签分页
            {
                var tagID = await _generalTagService.GetTagIDByNameForArticle(tagName);
                if (tagID == Guid.Empty) { return RedirectPermanent($"/article/{locationCityCode}-1/"); }

                var finds = await articleService.PageByTagID(tagID, offset, limit);

                if (finds.Item2 > 0)
                {
                    articles = finds.Item1;
                    total = finds.Item2;
                }
            }
            else if (string.IsNullOrEmpty(search))
            {
                var result = this.articleService.GetStrictSlctArticles(userId, UUID, locationCityCode, offset, limit);
                articles = result.articles;
                total = result.total;
            }
            else
            {
                //走搜索引擎路径
                int pageno = (offset / limit) + 1;

                if (!string.IsNullOrWhiteSpace(search))
                {
                    if (User.Identity.IsAuthenticated)
                    {
                        var user = User.Identity.GetUserInfo();
                        _dataImport.SetHistory("PC", 4, search, locationCityCode, user.UserId, UUID);
                    }
                    else
                    {
                        _dataImport.SetHistory("PC", 4, search, locationCityCode, null, UUID);
                    }
                }
                var result = _dataSearch.SearchArticle(search, null, pageno, limit);
                var articleIds = result.Articles.Select(q => q.Id).ToArray();
                if (articleIds != null && articleIds.Any())
                {
                    articles = this.articleService.GetByIds(articleIds).OrderByDescending(a => a.time).ToList();
                }
                total = (int)result.Total;
            }
            ViewBag.CityName = await _cityCodeService.GetCityName(locationCityCode);
            //热门标签
            ViewBag.HotTags = await articleService.GetHotTags();
            //热门城市
            var hotCity = await _cityCodeService.GetHotCity();
            ViewBag.HotCity = hotCity;

            //猜你喜欢
            //var youwilllike = await this.articleService.HotPointArticles(locationCityCode, UserId, 5, 10);
            //热门攻略
            var hotPoint = await this.articleService.HotPointArticles(locationCityCode, UserId, 0, 50);

            var randomArticals = hotPoint.articles.ToList();
            if (hotPoint.articles.Count() > 0 && hotPoint.articles.Count() < 50)
            {
                var leftArticals = await articleService.HotPointArticles(0, UserId, hotPoint.articles.Count(), 50 - hotPoint.articles.Count());
                if (leftArticals.articles.Count() > 0)
                {
                    randomArticals.AddRange(leftArticals.articles.ToList());
                }
            }
            CommonHelper.ListRandom(randomArticals);

            randomArticals = randomArticals.Where(p => p.time > DateTime.Now.AddDays(-90)).ToList();//排除90天外的

            //var allAIds = articles.Select(a => a.id)
            //                .Union(youwilllike.articles.Select(a => a.id))
            //                .Union(hotPoint.articles.Select(a => a.id));
            var allAIds = randomArticals.Select(p => p.id).Distinct().ToList();
            allAIds.AddRange(articles?.Select(q => q.id).Distinct() ?? new List<Guid>());
            var covers = this.articleCoverService.GetCoversByIds(allAIds.ToArray());

            ViewBag.IThinkYouLike = randomArticals.Take(10).Select(a => new ViewModels.Article.ArticleListItemViewModel()
            {
                Id = a.id,
                No = UrlShortIdUtil.Long2Base32(a.No),
                Layout = a.layout,
                Type = string.IsNullOrEmpty(a.type) ? 1 : int.Parse(a.type),
                Title = a.title,
                ViewCount = a.VirualViewCount,
                Time = ArticleListItemTimeFormart(a.time.GetValueOrDefault()),
                Covers = covers.Where(c => c.articleID == a.id).Select(c => Article_CoverToLink(c)).ToList()
            });

            ViewBag.HotPoint = randomArticals.Skip(10).Take(5).Select(a => new ViewModels.Article.ArticleListItemViewModel()
            {
                Id = a.id,
                No = UrlShortIdUtil.Long2Base32(a.No),
                Layout = a.layout,
                Type = string.IsNullOrEmpty(a.type) ? 1 : int.Parse(a.type),
                Title = a.title,
                ViewCount = a.VirualViewCount,
                Time = ArticleListItemTimeFormart(a.time.GetValueOrDefault()),
                Covers = covers.Where(c => c.articleID == a.id).Select(c => Article_CoverToLink(c)).ToList()
            });
            ViewBag.Search = search;
            ViewBag.Offset = offset;
            ViewBag.Limit = limit;
            ViewBag.Total = total;
            ViewBag.TotalPage = (int)Math.Ceiling((double)total / limit);
            ViewData.Model = articles?.Select(a =>
            {
                return new ViewModels.Article.ArticleListItemViewModel()
                {
                    Id = a.id,
                    No = UrlShortIdUtil.Long2Base32(a.No),
                    Layout = a.layout,
                    Type = string.IsNullOrEmpty(a.type) ? 1 : int.Parse(a.type),
                    Title = a.title,
                    ViewCount = a.VirualViewCount,
                    Time = ArticleListItemTimeFormart(a.time.GetValueOrDefault()),
                    Covers = covers.Where(c => c.articleID == a.id).Select(c => Article_CoverToLink(c)).ToList()
                };
            });

            //热门点评列表【侧边栏】
            Guid CQUserId = userId ?? Guid.Empty;
            //var date_Now = DateTime.Now;
            //ViewBag.HottestComment = hot.HotComment(await _commentService.HotComment(new HotCommentQuery()
            //{
            //    Condition = true,
            //    City = 310100,
            //    Grade = 1,
            //    Type = 1,
            //    Discount = false,
            //    Diglossia = false,
            //    Chinese = false,
            //    StartTime = date_Now.AddMonths(-1),
            //    EndTime = date_Now
            //}, CQUserId), CQUserId);

            //热问
            //ViewBag.HottestSchoolQuesitonView = hot.HotQuestion(await questionInfoService.GetHotQuestion());

            ViewBag.CQUserId = CQUserId;
            ViewBag.LocalCityCode = locationCityCode;
            ViewBag.TagName = tagName ?? "";
            return View();
        }

        /// <summary>
        /// 和articleId相关的学校点评
        /// </summary>
        /// <param name="articleId"></param>
        /// <returns></returns>
        [HttpGet]
        public ResponseResult GetCorrelationComments(Guid articleId)
        {
            //4.查询相关学部点评
            var scms = this.articleService.GetCorrelationSCMS(articleId, userId);

            #region 调用点评列表模块所用的逻辑
            var commentDtos = _commentService.QueryCommentByIds(scms.Select(x => x.Id).ToList(), UserId.GetValueOrDefault());
            var UserIds = new List<Guid>();
            UserIds.AddRange(commentDtos.GroupBy(q => q.UserId).Select(p => p.Key).ToList());
            var Users = _userService.ListUserInfo(UserIds.GroupBy(q => q).Select(p => p.Key).ToList());
            //点评分数实体【需要获取沈总的分数】
            var score = ScoreToStarHelper.CommentScoreToStar(commentDtos.Select(x => x.Score)?.ToList());
            var schoolExtIds = commentDtos?.Select(x => x.SchoolSectionId).ToList();
            //学校列表实体
            var schoolExts = _schoolInfoService.GetSchoolSectionByIds(schoolExtIds);
            //检测是否关注该点评
            List<Guid> checkCollection = _collectionService.GetCollection(commentDtos.Select(x => x.Id).ToList(), UserId.GetValueOrDefault());
            var comments = CommentDtoToVoHelper.CommentDtoToViewModel(_setting.QueryImager, commentDtos, UserHelper.UserDtoToVo(Users), score, SchoolDtoToSchoolCardHelper.SchoolDtoToSchoolCommentCard(schoolExts), checkCollection);
            #endregion
            return ResponseResult.Success(comments);
        }

        [Route("article/detail/{id}")]
        [Route("article/{no}.html")]
        [Description("文章详情")]
        public async Task<IActionResult> Detail(Guid id, string no, string sxbtoken = "")
        {
            if (id != Guid.Empty)
            {
                var article1 = this.articleService.GetShowForUserArticle(id, 0, sxbtoken.Equals("sxb007"));
                var numberId = UrlShortIdUtil.Long2Base32(article1.No);
                var redirectUrl = $"/article/{numberId.ToLower()}.html{Request.QueryString}";
                Response.Redirect(redirectUrl);
                Response.StatusCode = 301;
                return Json(new { });
            }
            double latitude = NumberParse.DoubleParse(Request.Cookies["latitude"], 0);
            double longitude = NumberParse.DoubleParse(Request.Cookies["longitude"], 0);
            int locationCityCode = HttpContext.Request.GetLocalCity();
            int no_int32 = (int)UrlShortIdUtil.Base322Long(no);
            var article = this.articleService.GetShowForUserArticle(id, no_int32, sxbtoken.Equals("sxb007"));
            if (article == null) { return NotFound(); }

            //热门标签
            ViewBag.HotTags = await articleService.GetHotTags();

            ViewBag.ArticleNo = no_int32;
            ViewBag.NearArticle = articleService.GetNearArticle(no_int32);

            //添加访问记录
            this.articleService.AddAccessCount(article);

            if (userId != null)
                _historyService.AddHistory(userId ?? Guid.Empty, article.id, (byte)MessageDataType.Article);

            //1.查绑定的群组图片
            var GQRCodes = this.articleService.GetCorrelationGQRCodes(article.id);


            //4.查询相关学部点评
            var scms = this.articleService.GetCorrelationSCMS(article.id, userId);

            #region 调用点评列表模块所用的逻辑
            var commentDtos = _commentService.QueryCommentByIds(scms.Select(x => x.Id).ToList(), UserId.GetValueOrDefault());
            var UserIds = new List<Guid>();
            UserIds.AddRange(commentDtos.GroupBy(q => q.UserId).Select(p => p.Key).ToList());
            var Users = _userService.ListUserInfo(UserIds.GroupBy(q => q).Select(p => p.Key).ToList());
            //点评分数实体【需要获取沈总的分数】
            var score = ScoreToStarHelper.CommentScoreToStar(commentDtos.Select(x => x.Score)?.ToList());
            var schoolExtIds = commentDtos?.Select(x => x.SchoolSectionId).ToList();
            //学校列表实体
            var schoolExts = _schoolInfoService.GetSchoolSectionByIds(schoolExtIds);
            //检测是否关注该点评
            List<Guid> checkCollection = _collectionService.GetCollection(commentDtos.Select(x => x.Id).ToList(), UserId.GetValueOrDefault());
            var comments = CommentDtoToVoHelper.CommentDtoToViewModel(_setting.QueryImager, commentDtos, UserHelper.UserDtoToVo(Users), score, SchoolDtoToSchoolCardHelper.SchoolDtoToSchoolCommentCard(schoolExts), checkCollection);
            #endregion

            //查询相关学校
            var correlationSchools = await this.GetCorrelationSchoolExt(this.articleService, article, latitude, longitude);

            List<PMS.OperationPlateform.Domain.DTOs.TagDto> correationTags = this.articleService.GetCorrelationTags(article.id, true);

            List<article> correlationArticles = this.articleService.GetCorrelationArticle(article.id);

            correlationArticles = correlationArticles.Where(p => p.time > DateTime.Now.AddDays(-90)).ToList();

            //热门攻略
            var hotPoint = await this.articleService.HotPointArticles(locationCityCode, UserId, 0, 50);

            var randomArticals = hotPoint.articles.ToList();
            if (hotPoint.articles.Count() > 0 && hotPoint.articles.Count() < 50)
            {
                var leftArticals = await articleService.HotPointArticles(0, UserId, hotPoint.articles.Count(), 50 - hotPoint.articles.Count());
                if (leftArticals.articles.Count() > 0)
                {
                    randomArticals.AddRange(leftArticals.articles.ToList());
                }
            }
            CommonHelper.ListRandom(randomArticals);

            randomArticals = randomArticals.Where(p => p.time > DateTime.Now.AddDays(-90)).ToList();//排除90天外的


            //热搜词
            List<string> hotKeyWord = _dataSearch.GetHotHistoryList(4, 8,null ,locationCityCode);

            ViewBag.HotKeyWord = hotKeyWord;

            var allAIds = hotPoint.articles.Select(a => a.id).Union(correlationArticles.Select(a => a.id));
            var covers = this.articleCoverService.GetCoversByIds(allAIds.ToArray());

            ViewBag.HotPoint = randomArticals.Take(5).Select(a => new ViewModels.Article.ArticleListItemViewModel()
            {
                Id = a.id,
                No = UrlShortIdUtil.Long2Base32(a.No),
                Layout = a.layout,
                Type = string.IsNullOrEmpty(a.type) ? 1 : int.Parse(a.type),
                Title = a.title,
                ViewCount = a.VirualViewCount,
                Time = ArticleListItemTimeFormart(a.time.GetValueOrDefault()),
                Covers = covers.Where(c => c.articleID == a.id).Select(c => Article_CoverToLink(c)).ToList()
            });
            var viewModel = new ViewModels.Article.ArticleViewModel()
            {
                Id = article.id,
                Time = article.time.GetValueOrDefault().ToString("yyyy-MM-dd"),
                Html = article.html,
                Title = article.title,
                ViweCount = article.VirualViewCount,
                Author = article.author,
                Digest = article.html.GetHtmlHeaderString(150)
            };

            viewModel.CorrelationArticle = correlationArticles.Select(a => new ViewModels.Article.ArticleListItemViewModel()
            {
                Id = a.id,
                No = UrlShortIdUtil.Long2Base32(a.No),
                Layout = a.layout,
                Type = string.IsNullOrEmpty(a.type) ? 1 : int.Parse(a.type),
                Title = a.title,
                ViewCount = a.VirualViewCount,
                Time = ArticleListItemTimeFormart(a.time.GetValueOrDefault()),
                Covers = covers.Where(c => c.articleID == a.id).Select(c => Article_CoverToLink(c)).ToList()
            });
            viewModel.CorrelationGQRCode = GQRCodes.ToList();
            viewModel.CorrelationTags = correationTags;
            viewModel.CorrelationSCMs = comments;
            viewModel.CorrelationSchoolExt = correlationSchools;

            Guid CQUserId = userId ?? Guid.Empty;

            //热门点评列表【侧边栏】
            //ViewBag.HottestComment = hot.HotComment(await _commentService.HotComment(new HotCommentQuery()
            //{
            //    Condition = true,
            //    City = 310100,
            //    Grade = 1,
            //    Type = 1,
            //    Discount = false,
            //    Diglossia = false,
            //    Chinese = false,
            //    StartTime = DateTime.Parse("2019-07-31"),
            //    EndTime = DateTime.Parse("2019-08-02")
            //}, CQUserId), CQUserId);

            ////热问
            //ViewBag.HottestSchoolQuesitonView = hot.HotQuestion(await questionInfoService.GetHotQuestion());

            ViewBag.CQUserId = CQUserId;
            ViewBag.LocalCityCode = locationCityCode;

            return View(viewModel);
        }



        [Description("提供列表里隐藏开放给seo的文章列表")]
        [Route("{controller}/hdlst")]
        public IActionResult GetHideInListArticles()
        {
            ViewData.Model = articleService.GetIsHideInLists().Select(a => {

                return new ArticleListItemViewModel()
                {
                    Id = a.id,
                    Time = ArticleListItemTimeFormart(a.time.GetValueOrDefault()),
                    Title = a.title,
                    ViewCount = a.VirualViewCount,
                    Layout = a.layout,
                    No = UrlShortIdUtil.Long2Base32(a.No),
                    Covers = new List<string>()
                };
            });

            return View();

        }


        /// <summary>
        /// 获取附近的学校
        /// </summary>
        /// <returns></returns>
        [Description("获取附近的学校")]
        public async Task<IActionResult> GetNearBySchools()
        {
            return AjaxSuccess(data: new SchExtDto0[0]); 

            //double latitude = NumberParse.DoubleParse(Request.Cookies["latitude"], 0);
            //double longitude = NumberParse.DoubleParse(Request.Cookies["longitude"], 0);
            ////附近学校(查询时间接近1s，在无高并发的情况下,决定采用异步请求的方式优化用户体验)
            //var result = await _schService.GetNearSchoolsBySchType((longitude, latitude));

            //if (result?.Count() > 0)
            //{
            //    var eids = result.Select(p => p.Eid).Distinct();
            //    var schoolNos = _schService.GetSchoolextNo(eids.ToArray());
            //    if (schoolNos.Count() > 0)
            //    {
            //        foreach (var item in schoolNos)
            //        {
            //            if (result.Any(p => p.Eid == item.Item1)) result.FirstOrDefault(p => p.Eid == item.Item1).SchoolNo = item.Item2;
            //        }
            //    }
            //}

            //return AjaxSuccess(data: result);
        }

        [Description("猜你喜欢加载更多")]
        [Route("api/[controller]/[action]")]
        public async Task<IActionResult> LoadMoreIThinkYouLike(int offset = 5, int limit = 10)
        {
            int locationCityCode = Request.GetLocalCity();
            //猜你喜欢
            var youwilllike = await this.articleService.HotPointArticles(locationCityCode, UserId, offset, limit);
            var allAIds = youwilllike.articles.Select(a => a.id);
            var covers = this.articleCoverService.GetCoversByIds(allAIds.ToArray());
            var model = youwilllike.articles.Select(a => new ViewModels.Article.ArticleListItemViewModel()
            {
                Id = a.id,
                No = UrlShortIdUtil.Long2Base32(a.No),
                Layout = a.layout,
                Type = string.IsNullOrEmpty(a.type) ? 1 : int.Parse(a.type),
                Title = a.title,
                ViewCount = a.VirualViewCount,
                Time = ArticleListItemTimeFormart(a.time.GetValueOrDefault()),
                Covers = covers.Where(c => c.articleID == a.id).Select(c => Article_CoverToLink(c)).ToList()
            });

            if (Request.IsAjax())
            {
                if (model.Any())
                {
                    return PartialView("ListItems", model);
                }
                else
                {
                    return Json(new { statu = -1 });
                }
            }
            else
            {
                //为了迎合seo，加一个页面处理
                ViewBag.offset = offset + limit;
                ViewBag.limit = limit;

                return View(model);
            }
        }

        /// <summary>
        /// 查询QACard的数据
        /// </summary>
        /// <param name="QID"></param>
        /// <returns></returns>
        [Description("获取问答卡片数据")]
        public IActionResult GetQACardData(Guid qaid)
        {
            var (question, sectionCommentCount) = this.questionInfoService.GetQuestionInfoById(qaid);
            if (question == null) { return Json(new { statu = -1, msg = "NotFound" }); }

            var userids = question.answer.Select(q => q.UserId).ToList();
            var users = _userService.ListUserInfo(userids);

            var schoolInfo = _schoolInfoService.QuerySchoolInfo(question.SchoolSectionId).Result;

            var QACard = new QACardViewModel()
            {
                QID = question.Id,
                replys = question.answer.Select(a => new QACardViewModel.Reply()
                {
                    id = a.Id,
                    content = a.AnswerContent,
                    uname = users.FirstOrDefault(q => q.Id == a.UserId)?.NickName
                }).ToList(),
                title = question.QuestionContent,
                replyTotalCount = question.AnswerCount,
                sname = schoolInfo.SchoolName,
                sqacount = sectionCommentCount
            };
            return Json(new
            {
                statu = 1,
                msg = "OK",
                data = QACard
            });
        }

        /// <summary>
        /// 发表问题回复
        /// </summary>
        /// <param name="content">回复内容</param>
        /// <param name="qId">问题ID</param>
        /// <returns></returns>

        [Authorize]
        [Description("发表问题回复")]
        public IActionResult SendQuestionReply(string content, Guid qId)
        {
            if (string.IsNullOrEmpty(content) || qId == default(Guid))
            {
                return Json(new { status = -1, msg = "参数错误" });
            }

            var contentCheck = _text.GarbageCheck(new ProductManagement.API.Aliyun.Model.GarbageCheckRequest()
            {
                scenes = new[] { "antispam" },
                tasks = new List<ProductManagement.API.Aliyun.Model.GarbageCheckRequest.Task>() {
                      new ProductManagement.API.Aliyun.Model.GarbageCheckRequest.Task(){
                       content= content,
                         dataId = Guid.NewGuid().ToString()
                      }
                      }
            }).Result;

            if (contentCheck.data.FirstOrDefault().results.FirstOrDefault().suggestion == "block")
            {
                return Json(new { status = -2, msg = "您发布的内容包含敏感词，<br>请重新编辑后再发布。" });
            }

            var userInfo = User.Identity.GetUserInfo();
            //todo.....
            var result = this.questionsAnswersInfoService.Insert(new PMS.CommentsManage.Domain.Entities.QuestionsAnswersInfo()
            {
                Id = Guid.NewGuid(),
                Content = content,
                CreateTime = DateTime.Now,
                UserId = userInfo.UserId,
                QuestionInfoId = qId,
            });
            if (result > 0)
            {
                return Json(new { status = 1, msg = "", data = new { uname = userInfo.Name, content = content } });
            }
            else
            {
                return Json(new { status = -1, msg = "发表失败." });
            }
        }

        /// <summary>
        /// 文章列表项的时间格式
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        private string ArticleListItemTimeFormart(DateTime dateTime)
        {
            var passBy_H = (int)(DateTime.Now - dateTime).TotalHours;

            if (passBy_H > 24 || passBy_H < 0)
            {
                //超过24小时,显示日期
                return dateTime.ToString("yyyy年MM月dd日");
            }
            if (passBy_H == 0)
            {
                return dateTime.ToString("刚刚");
            }
            else
            {
                return $"{passBy_H}小时前";
            }
        }

        /// <summary>
        /// 文章背景图转string 链接
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        private string Article_CoverToLink(article_cover c)

        {
            return !string.IsNullOrWhiteSpace(c.ImgUrl) ? c.ImgUrl : $"https://cos.sxkid.com/images/article/{c.articleID}/{c.photoID}.{((FileExtension)c.ext).ToString()}";
        }

        /// <summary>
        /// 查询相关学校
        /// </summary>
        /// <param name="articleService"></param>
        /// <param name="article"></param>
        /// <returns></returns>
        [Description("查询相关学校")]
        private async Task<List<SchoolExtListItemViewModel>> GetCorrelationSchoolExt(IArticleService articleService, PMS.OperationPlateform.Domain.Entitys.article article, double Latitude, double Longitude, int pageSize = 10)
        {
            var result = new List<SchoolExtListItemViewModel>();
            List<PMS.School.Application.ModelDto.SchoolExtFilterDto> schools = new List<PMS.School.Application.ModelDto.SchoolExtFilterDto>();
            if (article.ArticleType == ArticleType.Policy)
            {
                //政策文章(以地区和关联的学校类型来作学校和文章的关联)
                var cra = this.articleService.GetCorrelationAreas(article.id).Select(a =>
                {
                    if (a.AreaId != null)
                        return int.Parse(a.AreaId);
                    else if (a.CityId != null)
                        return int.Parse(a.CityId);
                    else if (a.ProvinceId != null)
                        return int.Parse(a.ProvinceId);
                    else
                        return 0;
                }).ToList();

                var crst = this.articleService.GetCorrelationSchoolTypes(article.id).Select(a =>
                {
                    var schFT = new SchFType0((byte)a.SchoolGrade, (byte)a.SchoolType, a.Discount, a.Diglossia, a.Chinese);
                    return schFT.ToString();
                }).ToList();
                //调用搜索接口，搜寻关联学校
                var list = _dataSearch.SearchSchool(new SearchSchoolQuery
                {
                    Latitude = Latitude,
                    Longitude = Longitude,
                    CityCode = cra,
                    Type = crst,
                    Orderby = 5,
                    PageSize = pageSize
                });
                var sids = list.Schools.Select(q => q.Id).ToList();

                schools = sids.Count > 0 ? await this.schoolService.ListExtSchoolByBranchIds(sids) : new List<PMS.School.Application.ModelDto.SchoolExtFilterDto>();

            }
            else
            {
                //对比文章（直接由编辑人员作关联）
                var sids = articleService.GetCorrelationSchool(article.id).Select(s => s.SchoolId).ToList();
                if (sids.Count > 0)
                {

                    schools = await this.schoolService.ListExtSchoolByBranchIds(sids, Latitude, Longitude);


                }

            }

            //请求点评数据,榜单数据
            //拼接数据
            var rank = _schoolRankService.GetH5SchoolRankInfoBy(schools.Select(p => p.ExtId).ToArray());
            var comment = _commentService.GetSchoolCardComment(schools.Select(p => p.ExtId).ToList(),
                PMS.CommentsManage.Domain.Common.SchoolSectionOrIds.SchoolSectionIds, default(Guid));

            result = schools.Select(s => new SchoolExtListItemViewModel()
            {
                SchoolId = s.Sid,
                BranchId = s.ExtId,
                SchoolName = s.Name,
                CityName = s.City,
                AreaName = s.Area,
                Distance = s.Distance,
                LodgingType = (int)s.LodgingType,
                LodgingReason = s.LodgingType.Description(),
                International = s.International,
                Cost = s.Tuition,
                Tags = s.Tags,
                Score = s.Score,
                CommentTotal = s.CommentCount,
                Type = s.Type,

                Ranks = rank.Where(p => p.SchoolId == s.ExtId).Select(p => new SchoolRankInfoViewModel
                {
                    RankId = p.RankId,
                    Cover = p.Cover,
                    Ranking = p.Ranking,
                    RankUrl = p.rank_url,
                    SchoolId = p.SchoolId,
                    Title = p.Title,
                    No = p.No
                }).ToList(),
                Comment = comment.FirstOrDefault(p => p.SchoolSectionId == s.ExtId)?.Content,
                CommontNo = UrlShortIdUtil.Long2Base32(comment.FirstOrDefault(p => p.SchoolSectionId == s.ExtId)?.No ?? 0),
                SchoolNo = s.SchoolNo
            }).ToList(); ;
            return result;
        }

        /// <summary>
        /// 测试某个动作的运行时间需要多少
        /// </summary>
        /// <param name="action"></param>
        private void SubTest(Action action, string mark)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            action();
            stopwatch.Stop();
            TimeSpan time = stopwatch.Elapsed;
            Console.WriteLine($"{mark}:总花费时间：{time.TotalMilliseconds}");
        }
    }
}