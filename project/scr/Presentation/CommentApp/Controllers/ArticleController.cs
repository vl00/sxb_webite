using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Web.Controllers
{
    using HtmlAgilityPack;
    using iSchool;
    using iSchool.Internal.API.UserModule;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.Extensions.Logging;
    using PMS.CommentsManage.Application.IServices;
    using PMS.OperationPlateform.Application.IServices;
    using PMS.OperationPlateform.Domain.DTOs;
    using PMS.OperationPlateform.Domain.Entitys;
    using PMS.OperationPlateform.Domain.Enums;
    using PMS.School.Application.IServices;
    using PMS.Search.Application.IServices;
    using PMS.Search.Application.ModelDto.Query;
    using PMS.UserManage.Application.IServices;
    using ProductManagement.API.Aliyun;
    using ProductManagement.API.Http.Interface;
    using ProductManagement.Framework.Foundation;
    using Sxb.Web.Authentication.Attribute;
    using Sxb.Web.Common;
    using Sxb.Web.Filters.Attributes;
    using Sxb.Web.Utils;
    using Sxb.Web.ViewModels.School;
    using System.ComponentModel;
    using ViewModels.Article;
    using static PMS.UserManage.Domain.Common.EnumSet;

    /// <summary>
    /// 文章H5模块
    /// </summary>
    [UnValidateModel]
    public class ArticleController : BaseController
    {
        protected IArticleService articleService;
        private readonly ISchoolInfoService _schoolInfoService;

        private ISearchService _dataSearch;
        private ISchoolService schoolService;
        private IQuestionInfoService questionInfoService;
        private ISchoolCommentReplyService schoolCommentReplyService;
        private IQuestionsAnswersInfoService questionsAnswersInfoService;
        private IArticleCoverService articleCoverService;
        private ISearchService searchService;
        private IArticle_SubscribePreferenceServices article_SubscribePreferenceServices;
        private UserApiServices userApiServices;
        private ILocalV2Services localV2Services;
        private IUserServiceClient _userServiceClient;
        private ILogger logger;

        private readonly IText _text;
        private readonly IUserService _userService;

        public ArticleController(IArticleService articleService
            , ISchoolService schoolService
            , ISchoolInfoService schoolInfoService
            , IQuestionInfoService questionInfoService
            , IUserService userService
            , ISchoolCommentReplyService schoolCommentReplyService
            , IQuestionsAnswersInfoService questionsAnswersInfoService
            , IArticleCoverService articleCoverService
            , ISearchService searchService
            , IArticle_SubscribePreferenceServices article_SubscribePreferenceServices
            , UserApiServices userApiServices
            , ILocalV2Services localV2Services
            , IUserServiceClient userServiceClient
            , ILogger<ArticleController> logger
            , IText text
            , ISearchService dataSearch

            )

        {
            this._dataSearch = dataSearch;
            this.articleService = articleService;
            this.schoolService = schoolService;
            this.questionInfoService = questionInfoService;
            this._userService = userService;
            this.schoolCommentReplyService = schoolCommentReplyService;
            this.questionsAnswersInfoService = questionsAnswersInfoService;
            this.articleCoverService = articleCoverService;
            this.searchService = searchService;
            this.article_SubscribePreferenceServices = article_SubscribePreferenceServices;
            this.userApiServices = userApiServices;
            this.localV2Services = localV2Services;
            this._userServiceClient = userServiceClient;
            this.logger = logger;
            this._text = text;

            _schoolInfoService = schoolInfoService;
        }

        /// <summary>
        /// 文章列表 tagType 0-原创 1-订阅
        /// </summary>
        [Route("{controller}/{action=List}")]
        [Route("{controller}/{cityCode}-{pageIndex}")]
        [Description("文章列表")]
        public IActionResult List(int tabIndex, bool ihp = true, int offset = 0, int limit = 20, int cityCode = default, int pageIndex = default)
        {
            string uuid = Request.GetDevice();
            var locationCityCode = Request.GetLocalCity() == 0 ? Request.GetCity() : Request.GetLocalCity();
            if (cityCode != default) locationCityCode = cityCode;

            var result = this.articleService.GetStrictSlctArticles(userId, uuid, locationCityCode, offset, limit);
            var covers = this.articleCoverService.GetCoversByIds(result.articles.Select(a => a.id).ToArray());
            var ogaData = result.articles.Select(a => new ArticleListItemViewModel()
            {
                Id = a.id,
                Time = ArticleListItemTimeFormart(a.time.GetValueOrDefault()),
                Title = a.title,
                ViweCount = a.VirualViewCount,
                Covers = covers.Where(c => c.articleID == a.id).Select(c => $"https://cdn.sxkid.com/images/article/{c.articleID}/{c.photoID}.{((FileExtension)c.ext).ToString()}").ToList(),
                Layout = a.layout,
                Digest = a.html.GetHtmlHeaderString(150),
                No = UrlShortIdUtil.Long2Base32(a.No)
            });
            ViewBag.UA = Request.GetClientType();
            if (Request.IsAjax())
            {
                if (ogaData == null || !ogaData.Any())
                {
                    return Json(new { statu = 2 });
                }

                return PartialView("~/Views/Article/_Original.cshtml", ogaData);
            }
            else
            {
                ViewBag.Offset = offset;
                ViewBag.Limit = limit;
                ViewBag.TabIndex = tabIndex;
                ViewBag.OGAData = ogaData;
                ViewBag.isHomePage = ihp;
                return View();
            }
        }

        /// <summary>
        /// 订阅的文章列表页
        /// </summary>
        /// <returns></returns>
        [Description("订阅的文章列表页")]
        public async Task<IActionResult> SubscribeList(int type, int offset = 0, int limit = 20)
        {
            //参数检查
            offset = offset < 0 ? 0 : offset;

            //获取客户端定位、设备标识/用户标识
            var locationCityCode = Request.GetCity() == 0 ? Request.GetLocalCity() : Request.GetCity();
            Guid prefernceUserId = userId == null ? Request.GetDeviceToGuid() : userId.Value;

            //查询该设备或者该用户的订阅偏好记录
            var preference = this.article_SubscribePreferenceServices.GetByUserId(prefernceUserId);
            //如果没有偏好记录，尝试构建一个
            if (preference == null)
            {
                //初始一个
                preference = new Article_SubscribePreference();
                //初始化数据来源于：开机问卷
                var interestInfo = await this.userApiServices.GetUserInterestAsync(userId, Request.GetDevice());
                //问卷中记录了学校等级和学校类型
                preference.Grades = string.Join(',', interestInfo.grade);
                preference.SchoolTypes = string.Join(',', interestInfo.nature);
                //用户ID存用户ID或者设备ID
                preference.UserId = prefernceUserId;
                //城市使用当前定位城市
                var city = this.localV2Services.GetById(locationCityCode);
                if (city != null)
                {
                    preference.ProvinceId = city.parent;
                    preference.CityId = city.id;
                }
                //是否推送关注学校的文章，默认为不推荐
                preference.IsPushSubscibeSchool = false;
                //最后保存用户偏好
                preference = this.article_SubscribePreferenceServices.Add(preference);
                if (preference == null) //如果新增不成功，那么记录日志，终结流程
                {
                    var exception = new Exception("在查看攻略订阅时，为用户添加订阅偏好发生了异常。");
                    this.logger.LogError(exception, null, null);
                    throw exception;
                }
            }

            preference.Province = this.localV2Services.GetById(preference.ProvinceId.GetValueOrDefault());
            preference.City = this.localV2Services.GetById(preference.CityId.GetValueOrDefault());
            preference.Area = this.localV2Services.GetById(preference.AreaId.GetValueOrDefault());
            //获取订阅文章列表
            var subscribeAritcles = await this.articleService.GetSubscribeArticles(preference, HttpContext.Request.Cookies["iSchoolAuth"], offset, limit);
            var outputArticles = subscribeAritcles.Select(a => new ArticleListItemViewModel()
            {
                Id = a.id,
                Title = a.title,
                Layout = a.layout,
                Time = ArticleListItemTimeFormart(a.time.GetValueOrDefault()),
                Digest = a.html.GetHtmlHeaderString(150),
                ViweCount = a.VirualViewCount,
                Covers = a.Covers.Select(c => Article_CoverToLink(c)).ToList(),
                No = UrlShortIdUtil.Long2Base32(a.No)
            });
            if (type == 0)
            {
                //请求订阅初始页
                ViewBag.Articles = outputArticles;
                ViewBag.Provinces = this.localV2Services.GetByParent(0);  //初始省份数据
                if (preference.Province != null)
                {
                    ViewBag.Citys = this.localV2Services.GetByParent(preference.Province.id);
                    if (preference.City != null)
                    {
                        ViewBag.Areas = this.localV2Services.GetByParent(preference.City.id);
                    }
                }
                ViewBag.Offset = offset;
                ViewBag.Limit = limit;
                return PartialView(preference);
            }
            else
            {
                //请求翻页数据
                if (outputArticles == null || !outputArticles.Any())
                {
                    return Json(new { statu = 2 });
                }
                return PartialView("~/Views/Article/_Original.cshtml", outputArticles);
            }
        }



        [Description("提供列表里隐藏开放给seo的文章列表")]
        [Route("{controller}/hdlst")]
        public IActionResult GetHideInListArticles()
        {
           ViewData.Model =   articleService.GetIsHideInLists().Select(a=> {

               return new ArticleListItemViewModel()
               {
                   Id = a.id,
                   Time = ArticleListItemTimeFormart(a.time.GetValueOrDefault()),
                   Title = a.title,
                   ViweCount = a.VirualViewCount,
                   Layout = a.layout,
                   Digest = a.html.GetHtmlHeaderString(150),
                   No = UrlShortIdUtil.Long2Base32(a.No),
                   Covers = new List<string>()
               };
           });

            return View();

        }

        /// <summary>
        /// 学校动态文章列表
        /// </summary>
        /// <param name="schoolId"></param>
        /// <returns></returns>
        [Description("学校动态文章列表")]
        public IActionResult ListSchoolDynamic(Guid schoolId, int offset = 0, int limit = 20)
        {
            var result = this.articleService.GetComparisionArticles_PageVersion(new List<Guid>() { schoolId }, offset, limit);

            var models = result.articles.Select(a => new ArticleListItemViewModel()
            {
                Id = a.id,
                Time = ArticleListItemTimeFormart(a.time.GetValueOrDefault()),
                CommentCount = 0,
                Covers = null,
                Digest = a.html.GetHtmlHeaderString(150),
                Layout = a.layout,
                Title = a.title,
                ViweCount = a.VirualViewCount,
                No = UrlShortIdUtil.Long2Base32(a.No)
            });
            if (HttpContext.Request.IsAjax())
            {
                return base.AjaxSuccess(data: new { List = models, total = result.total });
            }
            else
            {
                ViewBag.Total = result.total;
                ViewBag.SchoolId = schoolId;
                ViewBag.Offset = offset;
                ViewBag.Limit = limit;
                return View(models);
            }
        }

        [Description("学校政策列表")]
        public IActionResult ListCreersPolicy(
            PMS.OperationPlateform.Domain.DTOs.SchoolType schoolType,
            Local local,
            int offset = 0,
            int limit = 20
            )
        {
            var result = this.articleService.GetPolicyArticles_PageVersion(new List<PMS.OperationPlateform.Domain.DTOs.SchoolType>() {
                 schoolType
            },
             new List<Local>() {
                 local
             },
             offset,
             limit
            );

            var articleIds = result.articles.Select(a => a.id).ToArray();
            var covers = this.articleCoverService.GetCoversByIds(articleIds);

            var models = result.articles.Select(a => new ArticleListItemViewModel()
            {
                Id = a.id,
                Time = ArticleListItemTimeFormart(a.time.GetValueOrDefault()),
                CommentCount = 0,
                Covers = covers.Where(c => c.articleID == a.id).Select(c => Article_CoverToLink(c)).ToList(),
                Digest = a.html.GetHtmlHeaderString(150),
                Layout = a.layout,
                Title = a.title,
                ViweCount = a.VirualViewCount,
                No = UrlShortIdUtil.Long2Base32(a.No)
            });

            if (HttpContext.Request.IsAjax())
            {
                return base.AjaxSuccess(data: new { List = models, total = result.total });
            }
            else
            {
                ViewBag.Total = result.total;
                ViewBag.Type = schoolType.Type;
                ViewBag.SchoolGrade = schoolType.SchoolGrade;
                ViewBag.Discount = Convert.ToInt16(schoolType.Discount);
                ViewBag.Diglossia = Convert.ToInt16(schoolType.Diglossia);
                ViewBag.Chinese = Convert.ToInt16(schoolType.Chinese);
                ViewBag.ProvinceId = local.ProvinceId;
                ViewBag.CityId = local.CityId;
                ViewBag.AreaId = local.AreaId;
                ViewBag.Offset = offset;
                ViewBag.Limit = limit;
                return View(models);
            }
        }

        /// <summary>
        /// 文章详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Description("文章详情")]
        [Route("article/{no}.html")]
        [Route("article/detail/{id}")]
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


            _ = _userServiceClient.AddHistory(new ProductManagement.API.Http.Model.AddHistory() { dataID = article.id, dataType = MessageDataType.Article, cookies = HttpContext.Request.GetAllCookies() });
            var firstCover = this.articleCoverService.GetCoversByIds(new[] { article.id }).FirstOrDefault();
            ViewBag.Cover = firstCover == null ? "https://cdn.sxkid.com/images/logo_share_v4.png" : Article_CoverToLink(firstCover);
            //检测当前用户是否为校方用户
            bool isSchoolRole = false;

            if (User.Identity.IsAuthenticated)
            {
                if (User.Identity.GetUserInfo().Role.Contains((int)PMS.UserManage.Domain.Common.UserRole.School))
                {
                    isSchoolRole = true;
                }
            }

            ViewBag.isSchoolRole = isSchoolRole;

            //1.查绑定的群组图片
            var GQRCodes = this.articleService.GetCorrelationGQRCodes(article.id);

            //2.查询绑定的标签
            var tags = this.articleService.GetCorrelationTags(article.id, true);

            //3.查询相关文章
            var articles = this.articleService.GetCorrelationArticle(article.id);

            Guid? userId = User.Identity.IsAuthenticated ? new Nullable<Guid>(User.Identity.GetUserInfo().UserId) : null;

            //4.查询相关学部点评
            var scms = this.articleService.GetCorrelationSCMS(article.id, userId);
            if (scms != null && scms.Any())
            {

                //查找学校
                var schoolInfos = _schoolInfoService.GetSchoolSectionByIds(scms.Select(x => x.SchoolExtId)?.ToList());

                //查找点评的学校
                scms.ForEach(s =>
                {

                    var info = schoolInfos.Where(x => s.SchoolExtId == x.SchoolSectionId).FirstOrDefault();
                    if (info != null)
                    {
                        s.SchoolName = info.SchoolName;
                        s.SchoolCommentCount = info.SectionCommentTotal;
                        s.SchoolScore = info.SchoolAvgScore;
                        s.SchoolStarts = info.SchoolStars;
                        s.SchoolAvgScore = Math.Round(info.SchoolAvgScore / 20, 1);
                        s.SchoolNo = info.SchoolNo;
                    }
                    else
                    {
                        s.SchoolName = "暂无该学校";
                        s.SchoolCommentCount = 0;
                        s.SchoolScore = 0;
                    }
                });
            }

            //5.查询相关学校
            var schools = await this.GetCorrelationSchoolExt(this.articleService, article, latitude, longitude);

            //6.添加访问量
            this.articleService.AddAccessCount(article);

            //为了配合seo，需要对正文内容清洗
            var doc = new HtmlDocument();
            doc.LoadHtml(article.html);
            //清洗a标签，外链链接需要给a标签加上rel：nofollow防止爬虫跳转
            var anodes = doc.DocumentNode.SelectNodes("//a[not(contains(@href,\"sxkid.com\"))]");
            if (anodes != null)
            {
                foreach (var node in anodes)
                {
                    node.SetAttributeValue("rel", "nofollow");
                }
                article.html = doc.DocumentNode.OuterHtml;
            }


            var viewModel = new ViewModels.Article.ArticleDetailViewModel()
            {
                Id = article.id,
                Time = article.time.GetValueOrDefault().ToString("yyyy年MM月dd日"),
                Html = article.html,
                Title = article.title,
                ViweCount = article.VirualViewCount,
                Author = article.author.Contains("上学帮") ? "上学帮" : article.author,
                Digest = article.html.GetHtmlHeaderString(150),
                No = article.No
            };

            //7.查询第一所学校分部所属学校的旗下分部
            var first_sext = (await this.GetCorrelationSchoolExt(this.articleService, article, latitude, longitude, 1)).FirstOrDefault();
            if (first_sext != null)
            {
                viewModel.FirstSchoolBranchs = this.articleService.GetSchoolExtensionByParent(first_sext.SchoolId).ToList();
            }

            viewModel.CorrelationGQRCode = GQRCodes.ToList();
            viewModel.CorrelationTags = tags.ToList();
            viewModel.CorrelationArticle = articles.Select(a => new ArticleListItemViewModel()
            {
                Id = a.id,
                Time = ArticleListItemTimeFormart(a.time.GetValueOrDefault()),
                Layout = a.layout,
                Title = a.title,
                ViweCount = a.VirualViewCount,
                Covers = a.Covers.Select(c => $"https://cdn.sxkid.com/images/article/{c.articleID}/{c.photoID}.{((FileExtension)c.ext).ToString()}").ToList(),
                No = UrlShortIdUtil.Long2Base32(a.No)
            }).ToList();
            viewModel.CorrelationSCMs = scms;
            viewModel.CorrelationSchoolExt = schools.Select(q => new SchoolExtListItemViewModel
            {
                SchoolId = q.SchoolId,
                BranchId = q.BranchId,
                SchoolName = q.SchoolName,
                CityName = q.CityName,
                AreaName = q.AreaName,
                Distance = q.Distance,
                LodgingType = q.LodgingType,
                LodgingReason = q.LodgingReason,
                Type = q.Type,
                International = q.International,
                Cost = q.Cost,
                Tags = q.Tags,
                Score = q.Score,
                CommentTotal = q.CommentTotal,
                SchoolNo = q.SchoolNo
            });

            return View(viewModel);
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
                sqacount = sectionCommentCount,
                ShortNo = UrlShortIdUtil.Long2Base32(question.No).ToLower()
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
        [BindMobile]
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
        /// 获取地区信息
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        [Description("获取地区信息")]
        public IActionResult GetLocals(int pid)
        {
            var locals = this.articleService.GetSubLocal(pid).Select(local => new
            {
                local.id,
                local.name
            });
            return Json(new
            {
                statu = 1,
                data = locals
            });
        }

        [Description("保存用户订阅配置")]
        public IActionResult SaveSubscribePreference(
          int province,
          int city,
          int area,
          SchoolGrade[] grade,
          PMS.OperationPlateform.Domain.Enums.SchoolType[] schoolType,
          bool isPublish

            )
        {
            Guid prefernceUserId = userId == null ? Request.GetDeviceToGuid() : userId.Value;
            var preference = this.article_SubscribePreferenceServices.GetByUserId(prefernceUserId);
            if (preference == null)
            {
                preference = new Article_SubscribePreference();
                preference.UserId = prefernceUserId;
                preference.ProvinceId = province;
                preference.CityId = city;
                preference.AreaId = area;
                preference.Grades = grade == null ? null : string.Join(',', grade.Select(g => (int)g));
                preference.SchoolTypes = schoolType == null ? null : string.Join(',', schoolType.Select(g => (int)g));
                preference.IsPushSubscibeSchool = isPublish;
                preference = this.article_SubscribePreferenceServices.Add(preference);
            }
            else
            {
                preference.ProvinceId = province;
                preference.CityId = city;
                preference.AreaId = area;
                preference.Grades = grade == null ? null : string.Join(',', grade.Select(g => (int)g));
                preference.SchoolTypes = schoolType == null ? null : string.Join(',', schoolType.Select(g => (int)g));
                preference.IsPushSubscibeSchool = isPublish;
                preference = this.article_SubscribePreferenceServices.Update(preference, new string[] {
                    "ProvinceId","CityId","AreaId","Grades","SchoolTypes","IsPushSubscibeSchool"
                });
            }

            if (preference != null)
            {
                return Json(new { statu = 1, redirect_url = Url.Action("List", "Article", new { tabindex = 1 }) });
            }
            else
            {
                this.logger.LogError("");
                return Json(new { statu = -1 });
            }
        }

        [Description("文章页搜索学校")]
        public IActionResult SearchSchool(string searchName, int offset, int limit = 20)
        {
            if (limit <= 0) { limit = 20; }
            if (offset <= 0) { offset = 0; }

            var searchResult = this.searchService.SearchSchool(new PMS.Search.Application.ModelDto.Query.SearchSchoolQuery()
            {
                Keyword = searchName,
                PageSize = limit,
                PageNo = (offset / limit) + 1
            });

            return AjaxSuccess(data: searchResult.Schools.Select(s => new { s.Id, s.Name }));
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
        /// 搜索文章相关学校
        /// </summary>
        /// <param name="articleID"></param>
        /// <returns></returns>
        [Description("搜索文章相关学校")]
        public IActionResult SearchCorrelationSchool(Guid articleID, string searchValue)
        {
            var article = this.articleService.GetShowForUserArticle(articleID, 0);
            double latitude = NumberParse.DoubleParse(Request.Cookies["latitude"], 0);
            double longitude = NumberParse.DoubleParse(Request.Cookies["longitude"], 0);

            var schools = GetCorrelationSchoolExt(this.articleService, article, latitude, longitude, 10);
            return Json(new { status = 1, data = schools });
        }

        /// <summary>
        /// 查询相关学校
        /// </summary>
        /// <param name="articleService"></param>
        /// <param name="article"></param>
        /// <returns></returns>
        private async Task<List<SchoolExtDto>> GetCorrelationSchoolExt(IArticleService articleService, PMS.OperationPlateform.Domain.Entitys.article article, double Latitude, double Longitude, int pageSize = 10)
        {
            if (article.ArticleType == ArticleType.Policy)
            {
                //政策文章
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

                var list = _dataSearch.SearchSchool(new SearchSchoolQuery
                {
                    Latitude = Latitude,
                    Longitude = Longitude,
                    CityCode = cra,
                    Type = crst,
                    Orderby = 5,
                    PageSize = pageSize
                });
                var ids = list.Schools.Select(q => q.Id).ToList();
                List<PMS.School.Application.ModelDto.SchoolExtFilterDto> correlaionSchools = ids.Count > 0 ? await this.schoolService.ListExtSchoolByBranchIds(ids) : new List<PMS.School.Application.ModelDto.SchoolExtFilterDto>();
                return correlaionSchools.Select(s => new SchoolExtDto()
                {
                    SchoolNo = s.SchoolNo,
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
                    Type = s.Type
                }).OrderByDescending(sch => sch.Score).ToList();
            }
            else
            {
                //对比文章
                var sids = articleService.GetCorrelationSchool(article.id).Select(s => s.SchoolId).ToList();
                if (sids.Count > 0)
                {
                    var schools = this.schoolService.ListExtSchoolByBranchIds(sids, Latitude, Longitude).Result;
                    return schools.Select(s => new SchoolExtDto()
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
                        SchoolNo = s.SchoolNo
                    }).OrderByDescending(sch => sch.Score).ToList();
                }
                else
                {
                    return new List<SchoolExtDto>();
                }
            }
        }

        private string Article_CoverToLink(article_cover c)

        {
            if (c == null)
            {
                return "";
            }
            return $"https://cdn.sxkid.com/images/article/{c.articleID}/{c.photoID}.{((FileExtension)c.ext).ToString()}";
        }
    }
}